using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Saves HR's in-progress assessment as a draft, on the candidate only - nothing is written to
/// Hrms.Tbl_InterviewPanel. The draft moves RecommendationStatus off 'Pending' (which is what ends the Admin's
/// ability to act) but leaves IsHrAssessmentCompleted false, so HR can come back to it.</summary>
public record SaveHrAssessmentDraftCommand(Guid CandidateId, Guid HrEmployeeId, HrAssessmentDecisionDto Decision, Guid SavedBy) : IRequest<Result<string>>, ICandidateWorkflowCommand;

public class SaveHrAssessmentDraftCommandValidator : AbstractValidator<SaveHrAssessmentDraftCommand>
{
    public SaveHrAssessmentDraftCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
    }
}

internal sealed class SaveHrAssessmentDraftCommandHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SaveHrAssessmentDraftCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveHrAssessmentDraftCommand request, CancellationToken cancellationToken)
    {
        if (request.SavedBy == Guid.Empty || request.HrEmployeeId == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        if (candidate.IsHrAssessmentCompleted)
        {
            return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
        }

        var panel = await panelRepository.GetByCandidateIdAsync(request.CandidateId, includeEmployee: false);
        if (!panel.Any(p => p.IsSubmitted))
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment isn't available for this candidate.");
        }

        var decision = request.Decision;

        candidate.SaveHrAssessmentDraft(
            decision.RecommendationStatus ?? candidate.RecommendationStatus,
            decision.OverallPerformance,
            decision.SuitableRoleDepartment,
            decision.RecommendedGradeId,
            decision.IsTrainingRequired,
            request.SavedBy);

        candidate.SaveSalaryAndJoiningDetails(
            decision.CurrentSalary,
            decision.ExpectedSalary,
            decision.NoticePeriodInDays,
            decision.EarliestJoiningDate,
            decision.IsWillingRelocate,
            decision.PreferredLocation,
            request.SavedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("HR Assessment saved as draft successfully.");
    }
}
