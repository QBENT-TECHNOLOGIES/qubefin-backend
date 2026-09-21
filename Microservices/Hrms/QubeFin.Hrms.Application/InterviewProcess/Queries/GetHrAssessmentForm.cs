using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Called when HR opens the HR Assessment form for a candidate. Returns the average of every
/// submitted panelist's ratings (read-only in the form) plus whatever HR has already saved as a draft or
/// submission for this candidate (if any).</summary>
public record GetHrAssessmentFormQuery(Guid CandidateId, Guid HrEmployeeId) : IRequest<Result<HrAssessmentFormDto>>;

internal sealed class GetHrAssessmentFormQueryHandler(
    ICandidateRepository candidateRepository,
    IInterviewPanelRepository panelRepository) : IRequestHandler<GetHrAssessmentFormQuery, Result<HrAssessmentFormDto>>
{
    public async Task<Result<HrAssessmentFormDto>> Handle(GetHrAssessmentFormQuery request, CancellationToken cancellationToken)
    {
        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var allPanelEntries = await panelRepository.GetByCandidateIdAsync(request.CandidateId);
        var hrEntry = allPanelEntries.FirstOrDefault(p => p.EmployeeId == request.HrEmployeeId);
        var hrIsInterviewer = hrEntry.IsGenuineInterviewer();

        // The interviewers - everyone on the panel except HR's own administrative entry. Rows belonging to
        // the HR post are excluded too, so this matches what USP_GetInterviewCandidateById counts - except
        // when HR is genuinely scheduled on the panel (hrIsInterviewer), in which case their own submitted
        // score counts like anyone else's.
        var interviewerEntries = allPanelEntries
            .Where(p => p.EmployeeId != request.HrEmployeeId && !p.IsHrRow())
            .ToList();
        if (hrIsInterviewer)
        {
            interviewerEntries.Add(hrEntry!);
        }

        var submittedInterviewers = interviewerEntries.Where(p => p.IsSubmitted).ToList();

        if (submittedInterviewers.Count == 0)
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment isn't available for this candidate.");
        }

        var averages = HrAssessmentAverageCalculator.Compute(submittedInterviewers);

        var panelistSummaries = submittedInterviewers
            .Select(p => new PanelistRatingSummaryDto(
                p.EmployeeId,
                p.EmployeeCode ?? string.Empty,
                p.EmployeeName ?? string.Empty,
                p.Designation ?? string.Empty,
                p.AppearanceAttitudeRating,
                p.PersonalityRating,
                p.CommunicationRating,
                p.EducationRating,
                p.WorkExperienceRating,
                p.TechnicalCompetenceRating,
                p.FlexibilityRating,
                p.AmbitionRating,
                p.PotentialRating,
                p.OthersRating,
                p.TotalRatingPoint,
                p.IsRecommendedForPosition))
            .ToList();

        // Whether HR's *decision* has been finalized. When HR is also a genuine interviewer, hrEntry.IsSubmitted
        // reflects THEIR OWN interview submission, not the HR decision - so we key off RecommendationStatus
        // instead, the same signal Candidate.SubmitHrAssessment sets. For a pure (non-interviewer) HR,
        // hrEntry.IsSubmitted is still the right signal, exactly as before.
        var isHrDecisionSubmitted = hrIsInterviewer
            ? !string.IsNullOrEmpty(candidate.RecommendationStatus) && candidate.RecommendationStatus != "Pending"
            : hrEntry?.IsSubmitted ?? false;

        // The rating block: for a pure HR, once submitted the row is frozen and the stored snapshot is shown.
        // For a dual-role HR, hrEntry IS their own locked interviewer row - it is never overwritten with the
        // average - so always recompute live; it stays reproducible since the rows it's built from are
        // themselves locked once submitted.
        var showFrozenSnapshot = !hrIsInterviewer && isHrDecisionSubmitted;

        var dto = new HrAssessmentFormDto(
            request.CandidateId,
            isHrDecisionSubmitted,
            hrIsInterviewer,

            showFrozenSnapshot ? hrEntry!.AppearanceAttitudeRating : averages.AppearanceAttitudeRating,
            showFrozenSnapshot ? hrEntry!.PersonalityRating : averages.PersonalityRating,
            showFrozenSnapshot ? hrEntry!.CommunicationRating : averages.CommunicationRating,
            showFrozenSnapshot ? hrEntry!.EducationRating : averages.EducationRating,
            showFrozenSnapshot ? hrEntry!.WorkExperienceRating : averages.WorkExperienceRating,
            showFrozenSnapshot ? hrEntry!.TechnicalCompetenceRating : averages.TechnicalCompetenceRating,
            showFrozenSnapshot ? hrEntry!.FlexibilityRating : averages.FlexibilityRating,
            showFrozenSnapshot ? hrEntry!.AmbitionRating : averages.AmbitionRating,
            showFrozenSnapshot ? hrEntry!.PotentialRating : averages.PotentialRating,
            showFrozenSnapshot ? hrEntry!.OthersRating : averages.OthersRating,
            showFrozenSnapshot ? hrEntry!.TotalRatingPoint : averages.Total,

            candidate.OverallPerformance,
            candidate.SuitableRoleDepartment,
            candidate.RecommendedGradeId,
            candidate.IsTrainingRequired,
            candidate.RecommendationStatus,
            hrEntry?.AnyOtherJobsSuitedRemarks,
            hrEntry?.IsRecommendedForPosition,
            hrEntry?.PositiveRemarks,
            hrEntry?.NegativeRemarks,
            panelistSummaries);

        return Result.Ok(dto);
    }
}
