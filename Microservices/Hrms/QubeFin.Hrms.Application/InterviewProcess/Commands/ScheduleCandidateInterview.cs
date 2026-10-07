using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>The Schedule action on the candidate list: adds or moves the candidate's interview date and time.
/// HR, or the candidate's Admin (its creator), until the HR Assessment is completed. It creates nothing - no
/// panel, no interviewer, no interview letter. Panelists already on the panel who have not submitted are moved
/// to the new slot, so the Interview page keeps showing the candidate's interview date.</summary>
public record ScheduleCandidateInterviewCommand(Guid CandidateId, DateOnly InterviewDate, TimeOnly InterviewTime, Guid EmployeeId, Guid ModifiedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class ScheduleCandidateInterviewCommandValidator : AbstractValidator<ScheduleCandidateInterviewCommand>
{
    public ScheduleCandidateInterviewCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.InterviewDate).NotEmpty().WithMessage("Interview date is required.");
        RuleFor(x => x.InterviewDate)
            .GreaterThanOrEqualTo(_ => CandidateWorkflow.Today)
            .WithMessage("Interview date cannot be in the past.");
    }
}

internal sealed class ScheduleCandidateInterviewCommandHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository,
    QubeFinDataContext context,
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : IRequestHandler<ScheduleCandidateInterviewCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ScheduleCandidateInterviewCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty || request.EmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var hrPostId = Guid.Parse(configuration["HrPost"]!);
        if (!await context.IsHrEmployeeAsync(hrPostId, request.EmployeeId, cancellationToken) &&
            !await context.IsCandidateCreatorAsync(request.CandidateId, request.EmployeeId, cancellationToken))
        {
            return new ForbiddenError("Only HR or the candidate's creator can schedule the interview.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        if (candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("The interview cannot be rescheduled once the HR Assessment is completed.");
        }

        var isReschedule = candidate.InterviewDate is not null;

        candidate.Schedule(request.InterviewDate, request.InterviewTime, request.ModifiedBy);
        await candidateRepository.UpdateAsync(candidate);

        var panelists = (await panelRepository.GetByCandidateIdAsync(candidate.Id, includeEmployee: false)).Where(p => !p.IsSubmitted);
        foreach (var panelist in panelists)
        {
            panelist.Reschedule(request.InterviewDate, request.InterviewTime, request.ModifiedBy);
            await panelRepository.UpdateAsync(panelist);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(isReschedule ? "Interview rescheduled successfully." : "Interview scheduled successfully.");
    }
}
