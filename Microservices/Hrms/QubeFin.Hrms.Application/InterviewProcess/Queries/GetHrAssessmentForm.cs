using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Services;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

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

        var panel = await panelRepository.GetByCandidateIdAsync(request.CandidateId);
        var submitted = panel.Where(p => p.IsSubmitted).ToList();

        if (submitted.Count == 0)
        {
            return new ValidationError("No panelist has submitted their assessment yet - HR Assessment isn't available for this candidate.");
        }

        var averages = HrAssessmentAverageCalculator.Compute(submitted);

        var panelistSummaries = submitted
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

        var pendingPanelists = panel
            .Where(p => !p.IsSubmitted && !p.IsCandidateAbsent)
            .Select(p => new PendingPanelistDto(p.EmployeeId, p.EmployeeCode ?? string.Empty, p.EmployeeName ?? string.Empty, p.IsAcknowledged))
            .ToList();

        var dto = new HrAssessmentFormDto(
            request.CandidateId,
            candidate.IsHrAssessmentCompleted,

            averages.AppearanceAttitudeRating,
            averages.PersonalityRating,
            averages.CommunicationRating,
            averages.EducationRating,
            averages.WorkExperienceRating,
            averages.TechnicalCompetenceRating,
            averages.FlexibilityRating,
            averages.AmbitionRating,
            averages.PotentialRating,
            averages.OthersRating,
            averages.Total,

            candidate.OverallPerformance,
            candidate.SuitableRoleDepartment,
            candidate.RecommendedGradeId,
            candidate.IsTrainingRequired,
            candidate.RecommendationStatus,

            candidate.CurrentSalary,
            candidate.ExpectedSalary,
            candidate.NoticePeriodInDays,
            candidate.EarliestJoiningDate,
            candidate.IsWillingRelocate,
            candidate.PreferredLocation,

            panelistSummaries,
            pendingPanelists);

        return Result.Ok(dto);
    }
}
