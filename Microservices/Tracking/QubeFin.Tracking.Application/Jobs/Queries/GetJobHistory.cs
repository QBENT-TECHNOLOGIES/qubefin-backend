using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
public record GetJobHistoryQuery(Guid UserId, Guid JobId) : IRequest<Result<List<JobHistoryResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetJobHistoryQueryValidator : AbstractValidator<GetJobHistoryQuery>
{
    public GetJobHistoryQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record JobHistoryResponse(
    Guid Id,
    Guid JobId,
    JobHistoryAction Action,
    string PerformedByName,
    DateTime? PerformedOn,
    string? FromAssigneeName,
    string? ToAssigneeName,
    IReadOnlyList<string> TaskNos,
    Guid? RelatedJobId,
    string? RelatedJobCode,
    string? Remarks);
#endregion

#region --- HANDLER ---
internal sealed class GetJobHistoryQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository) : IRequestHandler<GetJobHistoryQuery, Result<List<JobHistoryResponse>>>
{
    public async Task<Result<List<JobHistoryResponse>>> Handle(GetJobHistoryQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var history = await jobRepository.GetJobHistoryAsync(viewer, request.JobId, TrackingClock.Today(), cancellationToken);
        if (history is null) return new RecordNotFoundError("Job not found");

        return Result.Ok(history.Select(h => new JobHistoryResponse(
            h.Id,
            h.JobId,
            Enum.Parse<JobHistoryAction>(h.Action),
            h.PerformedByName,
            h.PerformedOn,
            h.FromAssigneeName,
            h.ToAssigneeName,
            h.TaskNos,
            h.RelatedJobId,
            h.RelatedJobCode,
            h.Remarks)).ToList());
    }
}
#endregion
