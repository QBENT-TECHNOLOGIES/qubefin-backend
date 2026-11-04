using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Queries;

#region --- QUERY ---
public record GetTaskPointsQuery(Guid UserId, Guid? OrganisationUnitId, Guid? AssigneeId, bool PendingOnly) : IRequest<Result<List<JobTaskPointResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetTaskPointsQueryValidator : AbstractValidator<GetTaskPointsQuery>
{
    public GetTaskPointsQueryValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record JobTaskPointResponse(
    Guid JobId,
    string JobCode,
    string JobTitle,
    JobStatus JobStatus,
    DateOnly JobStartDate,
    DateOnly? JobEndDate,
    Guid? AssigneeId,
    string? AssigneeName,
    Guid TaskId,
    string TaskNo,
    string? TaskTitle,
    TaskType? TaskType,
    TaskPriority Priority,
    JobTaskStatus Status,
    TaskTargetKind? TargetKind,
    Guid? OriginRefId,
    Guid OrganisationUnitId,
    GeoPoint? TargetLocation,
    bool LocationMismatch);
#endregion

#region --- HANDLER ---
internal sealed class GetTaskPointsQueryHandler(ITrackingScopeRepository scopeRepository, IJobRepository jobRepository) : IRequestHandler<GetTaskPointsQuery, Result<List<JobTaskPointResponse>>>
{
    public async Task<Result<List<JobTaskPointResponse>>> Handle(GetTaskPointsQuery request, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(request.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var points = await jobRepository.GetTaskPointsAsync(viewer, request.OrganisationUnitId, request.AssigneeId, request.PendingOnly, TrackingClock.Today(), cancellationToken);

        return Result.Ok(points.Select(p => new JobTaskPointResponse(
            p.JobId,
            p.JobCode,
            p.JobTitle,
            Enum.Parse<JobStatus>(p.JobStatus),
            p.JobStartDate,
            p.JobEndDate,
            p.AssigneeId,
            p.AssigneeName,
            p.TaskId,
            p.TaskNo,
            p.TaskTitle,
            Enum.TryParse<TaskType>(p.TaskType, true, out var type) ? type : null,
            Enum.TryParse<TaskPriority>(p.Priority, true, out var priority) ? priority : TaskPriority.Medium,
            Enum.Parse<JobTaskStatus>(p.Status, true),
            Enum.TryParse<TaskTargetKind>(p.TargetKind, true, out var kind) ? kind : null,
            p.OriginRefId,
            p.OrganisationUnitId,
            p.TargetLocation,
            p.LocationMismatch)).ToList());
    }
}
#endregion
