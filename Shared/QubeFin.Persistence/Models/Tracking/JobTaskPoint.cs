namespace QubeFin.Persistence.Models.Tracking;

public record JobTaskPointRow(
    Guid JobId,
    string JobCode,
    string JobTitle,
    string JobStatus,
    DateOnly JobStartDate,
    DateOnly? JobEndDate,
    Guid? AssigneeId,
    string? AssigneeName,
    Guid TaskId,
    string TaskNo,
    string? TaskTitle,
    string TaskType,
    string? Priority,
    string Status,
    string? TargetKind,
    Guid? OriginRefId,
    Guid OrganisationUnitId,
    decimal? TargetLatitude,
    decimal? TargetLongitude,
    bool LocationMismatch);
