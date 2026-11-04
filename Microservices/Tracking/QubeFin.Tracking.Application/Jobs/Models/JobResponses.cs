using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;

namespace QubeFin.Tracking.Application.Jobs.Models;

public record JobSummaryResponse(
    Guid Id,
    string JobCode,
    string JobTitle,
    JobStatus Status,
    Guid? AssigneeId,
    string? AssigneeName,
    string AssigneeRole,
    string UnitName,
    IReadOnlyList<Guid> OrganisationUnitIds,
    IReadOnlyList<string> BranchNames,
    string CreatedByName,
    DateTime? CreatedOn,
    DateOnly StartDate,
    DateOnly? EndDate,
    int TaskCount,
    int DoneCount,
    int OpenCount,
    int SelfAddedCount,
    IReadOnlyList<TaskType> Types,
    Guid? TransferJobRefId,
    string? TransferJobRefCode)
{
    public static JobSummaryResponse From(JobSummaryRow row) => new(
        row.Id,
        row.JobCode,
        row.JobTitle,
        Enum.Parse<JobStatus>(row.Status),
        row.AssigneeId,
        row.AssigneeName,
        row.AssigneeRole,
        row.UnitName,
        row.OrganisationUnitIds,
        row.BranchNames,
        row.CreatedByName,
        row.CreatedOn,
        row.StartDate,
        row.EndDate,
        row.TaskCount,
        row.DoneCount,
        row.OpenCount,
        row.SelfAddedCount,
        row.Types.Select(t => Enum.TryParse<TaskType>(t, true, out var type) ? (TaskType?)type : null).OfType<TaskType>().ToList(),
        row.TransferJobRefId,
        row.TransferJobRefCode);
}

public record JobPermissions(bool CanTransfer, bool CanCancel, bool CanUpdate);

public record JobResponse(
    Guid Id,
    string JobCode,
    string JobTitle,
    JobStatus Status,
    Guid? AssigneeId,
    string? AssigneeName,
    string AssigneeRole,
    string UnitName,
    IReadOnlyList<Guid> OrganisationUnitIds,
    IReadOnlyList<string> BranchNames,
    string CreatedByName,
    DateTime? CreatedOn,
    DateOnly StartDate,
    DateOnly? EndDate,
    int TaskCount,
    int DoneCount,
    int OpenCount,
    int SelfAddedCount,
    IReadOnlyList<TaskType> Types,
    Guid? TransferJobRefId,
    string? TransferJobRefCode,
    string? JobDescription,
    Guid CreatedBy,
    List<JobTaskResponse> Tasks,
    JobPermissions Permissions)
{
    internal static JobResponse From(TrackingViewer viewer, JobDetailRow job)
    {
        var summary = JobSummaryResponse.From(job.Summary);
        return new JobResponse(
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
            summary.CreatedOn,
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
            job.Tasks.Select(JobTaskResponse.From).ToList(),
            JobPermissionRules.For(viewer, job));
    }
}

public record JobTaskTarget(
    TaskTargetKind? Kind,
    Guid? OriginRefId,
    string? TargetName,
    GeoPoint? TargetLocation,
    Guid OrganisationUnitId,
    string BranchName,
    int? RadiusMeters);

public record JobTaskVerification(
    GeoPoint? ArriveLocation,
    decimal? ArriveDistanceMeters,
    GeoPoint? CompletedLocation,
    decimal? CompletedDistanceMeter,
    bool IsLocationMatched,
    string? PhotoUrl);

public record JobTaskOdOutcome(
    bool IsOdMemberAvailable,
    bool IsEmireceived,
    decimal? ReceivedAmount,
    DateOnly? Odptpdate);

public record JobTaskResponse(
    Guid Id,
    string TaskNo,
    Guid JobId,
    TaskType? TaskType,
    string? TaskTitle,
    string? Notes,
    TaskPriority Priority,
    Guid? CoOrdinatedBy,
    string? CoOrdinatedByName,
    JobTaskTarget Target,
    JobTaskStatus? Status,
    TaskOrigin Origin,
    string? AddedByName,
    DateTime? TaskAddedOn,
    DateTime? ArrivedOn,
    DateTime? TaskCompletedOn,
    string? CompletedByName,
    string? Remarks,
    JobTaskVerification? Verification,
    JobTaskOdOutcome? OdOutcome,
    bool IsLate,
    Guid? TransferredToJobId,
    string? TransferredToJobCode,
    string? TaskCancelReason)
{
    public static JobTaskResponse From(JobTaskRow row)
    {
        var taskType = Enum.TryParse<TaskType>(row.TaskType, true, out var type) ? type : (TaskType?)null;
        var status = Enum.TryParse<JobTaskStatus>(row.Status, true, out var taskStatus) ? taskStatus : (JobTaskStatus?)null;
        var hasVerification = row.ArrivedOn.HasValue || row.ArriveLocation is not null || row.CompletedLocation is not null || row.PhotoUrl is not null;

        return new JobTaskResponse(
            row.Id,
            row.TaskNo,
            row.JobId,
            taskType,
            row.TaskTitle,
            row.Notes,
            Enum.TryParse<TaskPriority>(row.Priority, true, out var priority) ? priority : TaskPriority.Medium,
            row.CoOrdinatedBy,
            row.CoOrdinatedByName,
            new JobTaskTarget(
                Enum.TryParse<TaskTargetKind>(row.TargetKind, true, out var kind) ? kind : null,
                row.OriginRefId,
                row.TargetName,
                row.TargetLocation,
                row.OrganisationUnitId,
                row.BranchName,
                row.RadiusMeters),
            status,
            row.IsSelfAdded ? TaskOrigin.SelfAdded : TaskOrigin.Assigned,
            row.AddedByName,
            row.TaskAddedOn,
            row.ArrivedOn,
            row.TaskCompletedOn,
            row.CompletedByName,
            row.Remarks,
            hasVerification
                ? new JobTaskVerification(row.ArriveLocation, row.ArriveDistanceMeters, row.CompletedLocation, row.CompletedDistanceMeter, row.IsLocationMatched, row.PhotoUrl)
                : null,
            taskType == Enums.TaskType.OdFollowUp && status == JobTaskStatus.Completed
                ? new JobTaskOdOutcome(row.IsOdMemberAvailable, row.IsEmireceived, row.ReceivedAmount, row.Odptpdate)
                : null,
            row.IsLate,
            row.TransferredToJobId,
            row.TransferredToJobCode,
            row.TaskCancelReason);
    }
}
