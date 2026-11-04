using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;

namespace QubeFin.Tracking.Application.Jobs.Models;

// Job, JobTask and AddTasksResponse exactly as the requirements document names them (sections 6 and 7)

public record AddTasksResponse(JobDocResponse Job, List<SkippedTask> Skipped, List<TaskWarning> Warnings);

public record JobDocResponse(
    Guid Id,
    string JobNo,
    string Title,
    JobStatus Status,
    Guid? AssigneeId,
    string? AssigneeName,
    string AssigneeRole,
    string UnitName,
    IReadOnlyList<Guid> BranchIds,
    IReadOnlyList<string> BranchNames,
    string CreatedByName,
    DateTimeOffset? CreatedAt,
    DateOnly? StartFrom,
    DateOnly? CompleteBy,
    int TaskCount,
    int DoneCount,
    int OpenCount,
    int SelfAddedCount,
    IReadOnlyList<TaskType> Types,
    Guid? TransferredFromJobId,
    string? TransferredFromJobNo,
    string? Description,
    Guid CreatedById,
    List<JobTaskDocResponse> Tasks,
    JobPermissions Permissions)
{
    internal static JobDocResponse From(TrackingViewer viewer, JobDetailRow job)
    {
        var summary = JobSummaryResponse.From(job.Summary);
        return new JobDocResponse(
            summary.Id,
            summary.JobCode,
            summary.JobTitle,
            summary.Status,
            summary.AssigneeId,
            summary.AssigneeName,
            summary.AssigneeRole,
            summary.UnitName,
            summary.OrganisationUnitIds,
            summary.BranchNames,
            summary.CreatedByName,
            TrackingClock.WithIndiaOffset(summary.CreatedOn),
            summary.StartDate,
            summary.EndDate,
            summary.TaskCount,
            summary.DoneCount,
            summary.OpenCount,
            summary.SelfAddedCount,
            summary.Types,
            summary.TransferJobRefId,
            summary.TransferJobRefCode,
            job.JobDescription,
            job.CreatedBy,
            job.Tasks.Select(t => JobTaskDocResponse.From(JobTaskResponse.From(t), t)).ToList(),
            JobPermissionRules.For(viewer, job));
    }
}

public record JobTaskDocResponse(
    Guid Id,
    string TaskNo,
    Guid JobId,
    TaskType? Type,
    string? Title,
    string? Notes,
    TaskPriority Priority,
    Guid? AccompaniedById,
    string? AccompaniedByName,
    TaskTargetDocResponse Target,
    JobTaskStatus? Status,
    TaskOrigin Origin,
    string? AddedByName,
    DateTimeOffset? AddedAt,
    DateTimeOffset? ArrivedAt,
    DateTimeOffset? CompletedAt,
    string? CompletedByName,
    string? Remarks,
    TaskVerificationDocResponse? Verification,
    OdOutcomeDocResponse? OdOutcome,
    bool IsLate,
    Guid? TransferredToJobId,
    string? TransferredToJobNo,
    string? ClosedReason)
{
    internal static JobTaskDocResponse From(JobTaskResponse task, JobTaskRow row) => new(
        task.Id,
        task.TaskNo,
        task.JobId,
        task.TaskType,
        task.TaskTitle,
        task.Notes,
        task.Priority,
        row.CoOrdinatedByEmployeeId,
        task.CoOrdinatedByName,
        new TaskTargetDocResponse(
            task.Target.Kind,
            task.Target.OriginRefId,
            task.Target.TargetName,
            null,
            task.Target.TargetLocation,
            task.Target.OrganisationUnitId,
            task.Target.BranchName,
            task.Target.RadiusMeters),
        task.Status,
        task.Origin,
        task.AddedByName,
        TrackingClock.WithIndiaOffset(task.TaskAddedOn),
        TrackingClock.WithIndiaOffset(task.ArrivedOn),
        TrackingClock.WithIndiaOffset(task.TaskCompletedOn),
        task.CompletedByName,
        task.Remarks,
        task.Verification is null ? null : new TaskVerificationDocResponse(
            task.Verification.ArriveLocation,
            task.Verification.ArriveDistanceMeters,
            task.Verification.CompletedLocation,
            task.Verification.CompletedDistanceMeter,
            task.Verification.IsLocationMatched,
            task.Verification.PhotoUrl,
            null),
        task.OdOutcome is null ? null : new OdOutcomeDocResponse(
            task.OdOutcome.IsOdMemberAvailable,
            task.OdOutcome.IsEmireceived,
            task.OdOutcome.ReceivedAmount,
            task.OdOutcome.Odptpdate),
        task.IsLate,
        task.TransferredToJobId,
        task.TransferredToJobCode,
        task.TaskCancelReason);
}

// address and photoTakenAt have no column yet, so they are always null
public record TaskTargetDocResponse(
    TaskTargetKind? Kind,
    Guid? RefId,
    string? Name,
    string? Address,
    GeoPoint? Location,
    Guid BranchId,
    string BranchName,
    int? MatchRadiusMeters);

public record TaskVerificationDocResponse(
    GeoPoint? ArrivalLocation,
    decimal? ArrivalDistanceMeters,
    GeoPoint? CompletionLocation,
    decimal? CompletionDistanceMeters,
    bool? LocationMatched,
    string? PhotoUrl,
    DateTimeOffset? PhotoTakenAt);

public record OdOutcomeDocResponse(bool MemberAvailable, bool Paid, decimal? AmountCollected, DateOnly? PtpDate);
