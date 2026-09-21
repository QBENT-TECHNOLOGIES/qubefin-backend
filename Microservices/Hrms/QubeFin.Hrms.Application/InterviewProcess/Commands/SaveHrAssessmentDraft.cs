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
        var hrEntry = allPanelEntries.FirstOrDefault(p => p.EmployeeId == request.HrEmployeeId);
        var hrIsInterviewer = hrEntry.IsGenuineInterviewer();

        var submittedInterviewers = allPanelEntries
            .Where(p => p.EmployeeId != request.HrEmployeeId && !p.IsHrRow() && p.IsSubmitted)
            .ToList();
        if (hrIsInterviewer && hrEntry!.IsSubmitted)
        {
            submittedInterviewers.Add(hrEntry);
        }

        if (submittedInterviewers.Count == 0)
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment isn't available for this candidate.");
        }

        var decision = request.Decision;

        if (hrIsInterviewer)
        {
            // hrEntry is HR's own genuine interviewer row - it already holds their real score and their own
            // answers to IsRecommendedForPosition/PositiveRemarks/NegativeRemarks/AnyOtherJobsSuitedRemarks
            // via the ordinary interviewer submit/draft flow. Never overwrite it with the panel average here:
            // that would destroy HR's own interview record, and once they've submitted it, SaveAssessmentDraft
            // would simply fail (it's locked). The decision saved below only ever touches the Candidate
            // record, so saving it is never blocked by HR's own interviewer submission state.
        }
        else
        {
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

            if (hrEntry is null)
            {
                // Stamp HR's row with the candidate's own interview slot rather than "whenever HR happened to
                // open the form", so the row lines up with the interviewers' rows on the same candidate.
                var scheduledDate = candidate.InterviewDate;
                var scheduledTime = candidate.InterviewTime ?? TimeOnly.FromDateTime(DateTime.UtcNow);
                hrEntry = InterviewPanel.Schedule(request.CandidateId, request.HrEmployeeId, scheduledDate, scheduledTime, request.SavedBy);
                hrEntry.SaveAssessmentDraft(details, request.SavedBy);
                await panelRepository.AddRangeAsync(new[] { hrEntry }, cancellationToken);
            }
            else
            {
                if (!hrEntry.SaveAssessmentDraft(details, request.SavedBy))
                {
                    return new ValidationError("The HR Assessment has already been submitted and cannot be changed.");
                }

                await panelRepository.UpdateAsync(hrEntry);
            }
        }

        candidate.SaveHrAssessmentDraft(
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
