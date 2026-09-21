using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>Saves HR's in-progress assessment as a draft. The ten category ratings are always the
/// (re-)computed average of the submitted panelists - HR never edits them directly. Unlike Submit, this
/// does not touch Candidate.RecommendationStatus/TotalRatingPoint/RatingStatus, so the workflow does not
/// treat HR Assessment as complete until Submit is called.</summary>
public record SaveHrAssessmentDraftCommand(Guid CandidateId, Guid HrEmployeeId, HrAssessmentDecisionDto Decision, Guid SavedBy) : IRequest<Result<string>>;

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

        var allPanelEntries = await panelRepository.GetByCandidateIdAsync(request.CandidateId);

        // Interviewers are the AssessmentType = 'INTERVIEWER' rows - including HR's own row when HR is
        // genuinely scheduled on the panel. The HR Assessment row is never one of them.
        var interviewers = allPanelEntries.Interviewers().ToList();
        var submittedInterviewers = interviewers.Where(p => p.IsSubmitted).ToList();

        if (interviewers.Count == 0)
        {
            return new ValidationError("This candidate has no interview panel yet.");
        }

        if (submittedInterviewers.Count == 0)
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment isn't available for this candidate.");
        }

        var decision = request.Decision;

        // The averaged ratings are always recomputed from the submitted interviewers and stored on the HR
        // Assessment row. HR's own INTERVIEWER row (if any) is never touched here - it holds their real
        // interview score and is locked once they submitted it.
        var averages = HrAssessmentAverageCalculator.Compute(submittedInterviewers);
        var details = new AssessmentDetails(
            averages.AppearanceAttitudeRating, null,
            averages.PersonalityRating, null,
            averages.CommunicationRating, null,
            averages.EducationRating, null,
            averages.WorkExperienceRating, null,
            averages.TechnicalCompetenceRating, null,
            averages.FlexibilityRating, null,
            averages.AmbitionRating, null,
            averages.PotentialRating, null,
            averages.OthersRating, null,
            decision.AnyOtherJobsSuitedRemarks,
            decision.IsRecommendedForPosition,
            decision.PositiveRemarks,
            decision.NegativeRemarks);

        var hrRow = allPanelEntries.HrAssessmentRow();

        if (hrRow is null)
        {
            // Stamp the HR row with the candidate's own interview slot rather than "whenever HR happened to
            // open the form", so the row lines up with the interviewers' rows on the same candidate.
            var scheduledDate = candidate.InterviewDate;
            var scheduledTime = candidate.InterviewTime ?? TimeOnly.FromDateTime(DateTime.UtcNow);
            hrRow = InterviewPanel.CreateHrAssessmentRow(request.CandidateId, request.HrEmployeeId, scheduledDate, scheduledTime, request.SavedBy);
            hrRow.SaveAssessmentDraft(details, request.SavedBy); // also sets IsAttened = true
            hrRow.Acknowledge(request.SavedBy);
            await panelRepository.AddRangeAsync(new[] { hrRow }, cancellationToken);
        }
        else
        {
            if (!hrRow.SaveAssessmentDraft(details, request.SavedBy))
            {
                return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
            }

            // Saving the HR Assessment counts as HR acknowledging/attending the HR workflow.
            hrRow.Acknowledge(request.SavedBy);
            await panelRepository.UpdateAsync(hrRow);
        }

        candidate.SaveHrAssessmentDraft(
            decision.RecommendationStatus,
            decision.OverallPerformance,
            decision.SuitableRoleDepartment,
            decision.RecommendedGradeId,
            decision.IsTrainingRequired,
            request.SavedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("HR Assessment saved as draft successfully.");
    }
}
