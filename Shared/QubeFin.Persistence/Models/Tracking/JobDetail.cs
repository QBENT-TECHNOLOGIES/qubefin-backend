namespace QubeFin.Persistence.Models.Tracking;

// Where a user sits: a super admin, or the post and unit of their current designation
public record TrackingPosition(bool IsAdmin, string? PostName, Guid? UnitId);

public record JobDetailRow(
    JobSummaryRow Summary,
    string? JobDescription,
    Guid CreatedBy,
    TrackingPosition Creator,
    Guid AssignTo,
    TrackingPosition Assignee,
    List<JobTaskRow> Tasks);

public record JobTaskRow(
    Guid Id,
    string TaskNo,
    Guid JobId,
    string TaskType,
    string? TaskTitle,
    string? Notes,
    string? Priority,
    Guid? CoOrdinatedBy,
    Guid? CoOrdinatedByEmployeeId,
    string? CoOrdinatedByName,
    string? TargetKind,
    Guid? OriginRefId,
    string? TargetName,
    GeoPoint? TargetLocation,
    Guid OrganisationUnitId,
    string BranchName,
    int? RadiusMeters,
    string Status,
    bool IsSelfAdded,
    string? AddedByName,
    DateTime? TaskAddedOn,
    DateTime? ArrivedOn,
    GeoPoint? ArriveLocation,
    decimal? ArriveDistanceMeters,
    DateTime? TaskCompletedOn,
    string? CompletedByName,
    GeoPoint? CompletedLocation,
    decimal? CompletedDistanceMeter,
    bool IsLocationMatched,
    string? PhotoUrl,
    bool IsOdMemberAvailable,
    bool IsEmireceived,
    decimal? ReceivedAmount,
    DateOnly? Odptpdate,
    string? Remarks,
    bool IsLate,
    Guid? TransferredToJobId,
    string? TransferredToJobCode,
    string? TaskCancelReason);
