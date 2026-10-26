using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

public record RejectCandidateCommand(Guid CandidateId, Guid EmployeeId, Guid RejectedBy) : IRequest<Result<string>>;

public class RejectCandidateCommandValidator : AbstractValidator<RejectCandidateCommand>
{
    public RejectCandidateCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
    }
}

internal sealed class RejectCandidateCommandHandler(
    ICandidateRepository candidateRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : IRequestHandler<RejectCandidateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(RejectCandidateCommand request, CancellationToken cancellationToken)
    {
        if (request.RejectedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            if (!await context.IsCandidateCreatorAsync(request.CandidateId, request.EmployeeId, cancellationToken))
            {
                return new ForbiddenError("Only HR or the candidate's creator can reject a candidate.");
            }

            if (candidate.RecommendationStatus != CandidateWorkflow.Pending || candidate.IsHrAssessmentCompleted)
            {
                return new ForbiddenError("The HR Assessment has started - only HR can reject this candidate now.");
            }
        }

        var stoppedStatus = await context.GetStoppedStatusAsync(request.CandidateId, cancellationToken);
        if (stoppedStatus == CandidateInterviewStatus.Rejected)
        {
            return new ValidationError("This candidate has already been rejected.");
        }

        if (stoppedStatus == CandidateInterviewStatus.NotSelected)
        {
            return new ValidationError("This candidate was not selected. The workflow has already stopped.");
        }

        if (await candidateRepository.GetEmployeeIdAsync(request.CandidateId, cancellationToken) is not null)
        {
            return new ValidationError("This candidate has already joined as an employee and cannot be rejected.");
        }

        candidate.Reject(request.RejectedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"Candidate {candidate.FirstName} {candidate.LastName} has been rejected.");
    }
}
