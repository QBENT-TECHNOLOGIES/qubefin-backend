using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

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

        // Role is decided by AssessmentType, never by the employee's HR designation:
        //   - interviewers          = every AssessmentType = 'INTERVIEWER' row, HR's own included
        //   - hrRow                 = the single AssessmentType = 'HR' row, if HR has started one
        //   - hrIsInterviewer       = HR also holds an INTERVIEWER row on this candidate
        var interviewers = allPanelEntries.Interviewers().ToList();
        var hrRow = allPanelEntries.HrAssessmentRow();
        var hrOwnInterviewerRow = allPanelEntries.InterviewerRowFor(request.HrEmployeeId);
        var hrIsInterviewer = hrOwnInterviewerRow is not null;
        var isHrOnlyInterviewer = hrIsInterviewer && interviewers.Count == 1;

        var submittedInterviewers = interviewers.Where(p => p.IsSubmitted).ToList();

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

        // Whether HR's *decision* has been finalized. The HR row's IsSubmitted now belongs solely to the
        // HR Assessment workflow (HR's own interviewer submission lives on their separate INTERVIEWER row),
        // but RecommendationStatus stays the authoritative completion signal, matching
        // Candidate.SubmitHrAssessment and IsHrAssessmentCompleted in USP_GetInterviewCandidateById.
        var isHrDecisionSubmitted =
            (!string.IsNullOrEmpty(candidate.RecommendationStatus) && candidate.RecommendationStatus != "Pending")
            || (hrRow?.IsSubmitted ?? false);

        // Once submitted the HR row is frozen, so show the stored snapshot; until then show the live average.
        var showFrozenSnapshot = isHrDecisionSubmitted && hrRow is not null;

        var dto = new HrAssessmentFormDto(
            request.CandidateId,
            isHrDecisionSubmitted,
            hrIsInterviewer,
            isHrOnlyInterviewer,

            showFrozenSnapshot ? hrRow!.AppearanceAttitudeRating : averages.AppearanceAttitudeRating,
            showFrozenSnapshot ? hrRow!.PersonalityRating : averages.PersonalityRating,
            showFrozenSnapshot ? hrRow!.CommunicationRating : averages.CommunicationRating,
            showFrozenSnapshot ? hrRow!.EducationRating : averages.EducationRating,
            showFrozenSnapshot ? hrRow!.WorkExperienceRating : averages.WorkExperienceRating,
            showFrozenSnapshot ? hrRow!.TechnicalCompetenceRating : averages.TechnicalCompetenceRating,
            showFrozenSnapshot ? hrRow!.FlexibilityRating : averages.FlexibilityRating,
            showFrozenSnapshot ? hrRow!.AmbitionRating : averages.AmbitionRating,
            showFrozenSnapshot ? hrRow!.PotentialRating : averages.PotentialRating,
            showFrozenSnapshot ? hrRow!.OthersRating : averages.OthersRating,
            showFrozenSnapshot ? hrRow!.TotalRatingPoint : averages.Total,

            candidate.OverallPerformance,
            candidate.SuitableRoleDepartment,
            candidate.RecommendedGradeId,
            candidate.IsTrainingRequired,
            candidate.RecommendationStatus,
            hrRow?.AnyOtherJobsSuitedRemarks,
            hrRow?.IsRecommendedForPosition,
            hrRow?.PositiveRemarks,
            hrRow?.NegativeRemarks,
            panelistSummaries);

        return Result.Ok(dto);
    }
}
