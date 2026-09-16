using FluentResults;
using MediatR;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidateInterviewPanelsQuery(Guid CandidateId) : IRequest<Result<List<InterviewPanelDto>>>;

internal sealed class GetCandidateInterviewPanelsQueryHandler(IInterviewPanelRepository panelRepository)
    : IRequestHandler<GetCandidateInterviewPanelsQuery, Result<List<InterviewPanelDto>>>
{
    public async Task<Result<List<InterviewPanelDto>>> Handle(GetCandidateInterviewPanelsQuery request, CancellationToken cancellationToken)
    {
        var panels = await panelRepository.GetByCandidateIdAsync(request.CandidateId);

        var dtos = panels.Select(p => new InterviewPanelDto(
            p.Id,
            p.CandidateId,
            p.EmployeeId,
            p.ScheduledDate,
            p.ScheduledTime,
            p.IsAcknowledged,
            p.AcknowledgedDate,
            p.IsAttened,
            p.IsSubmitted,
            p.SubmissionDate,
            p.TotalRatingPoint,
            p.IsRecommendedForPosition)).ToList();

        return Result.Ok(dtos);
    }
}