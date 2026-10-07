using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>"Is Candidate Selected" after the HR Assessment: HR sets IsSelectedForOffer, which opens Candidate
/// Verification and the rest of the joining workflow. Only a qualified outcome can be selected, and the selection
/// is never reverted.</summary>
public record SelectCandidateForOfferCommand(Guid CandidateId, Guid EmployeeId, Guid ModifiedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class SelectCandidateForOfferCommandValidator : AbstractValidator<SelectCandidateForOfferCommand>
{
    public SelectCandidateForOfferCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
    }
}

internal sealed class SelectCandidateForOfferCommandHandler(
    ICandidateRepository candidateRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : IRequestHandler<SelectCandidateForOfferCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SelectCandidateForOfferCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR can select a candidate.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        if (!candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("Submit the HR Assessment before selecting the candidate.");
        }

        if (!CandidateWorkflow.IsQualified(candidate.RecommendationStatus))
        {
            return new ValidationError("Only a recommended candidate can be selected.");
        }

        if (candidate.IsSelectedForOffer)
        {
            return new ValidationError("This candidate has already been selected.");
        }

        candidate.SelectForOffer(request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"Candidate {candidate.FirstName} {candidate.LastName} has been selected. Candidate Verification is now open.");
    }
}
