using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

/// <summary>Returns a single panelist's interview assessment for a candidate.</summary>
public record GetInterviewAssessmentByCandidateAndEmployeeQuery(Guid CandidateId, Guid EmployeeId) : IRequest<Result<InterviewAssessmentDto>>;

internal sealed class GetInterviewAssessmentByCandidateAndEmployeeQueryHandler(IInterviewPanelRepository panelRepository)
    : IRequestHandler<GetInterviewAssessmentByCandidateAndEmployeeQuery, Result<InterviewAssessmentDto>>
{
    public async Task<Result<InterviewAssessmentDto>> Handle(GetInterviewAssessmentByCandidateAndEmployeeQuery request, CancellationToken cancellationToken)
    {
        var panel = await panelRepository.GetByCandidateAndEmployeeAsync(request.CandidateId, request.EmployeeId);
        if (panel is null)
        {
            return new RecordNotFoundError("Interview panel entry not found for the given candidate and employee.");
        }

        var dto = new InterviewAssessmentDto(
            panel.Id,
            panel.CandidateId,
            panel.EmployeeId,
            panel.EmployeeCode ?? string.Empty,
            panel.EmployeeName ?? string.Empty,
            panel.Designation ?? string.Empty,
            panel.IsAcknowledged,
            panel.AcknowledgedDate,
            panel.IsAttened,
            panel.IsSubmitted,
            panel.SubmissionDate,
            panel.AppearanceAttitudeRating, panel.AppearanceAttitudeRemarks,
            panel.PersonalityRating, panel.PersonalityRemarks,
            panel.CommunicationRating, panel.CommunicationRemarks,
            panel.EducationRating, panel.EducationRemarks,
            panel.WorkExperienceRating, panel.WorkExperienceRemarks,
            panel.TechnicalCompetenceRating, panel.TechnicalCompetenceRemarks,
            panel.FlexibilityRating, panel.FlexibilityRemarks,
            panel.AmbitionRating, panel.AmbitionRemarks,
            panel.PotentialRating, panel.PotentialRemarks,
            panel.OthersRating, panel.OthersRemarks,
            panel.TotalRatingPoint,
            panel.AnyOtherJobsSuitedRemarks,
            panel.IsRecommendedForPosition,
            panel.PositiveRemarks,
            panel.NegativeRemarks);

        return Result.Ok(dto);
    }
}
