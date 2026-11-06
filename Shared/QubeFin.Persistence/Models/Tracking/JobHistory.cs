namespace QubeFin.Persistence.Models.Tracking;

public record JobHistoryRow(
    Guid Id,
    Guid JobId,
    string Action,
    string PerformedByName,
    DateTimeOffset PerformedAt,
    string? FromAssigneeName,
    string? ToAssigneeName,
    IReadOnlyList<string> TaskNos,
    Guid? RelatedJobId,
    string? RelatedJobCode,
    DateOnly? OldCompleteBy,
    DateOnly? NewCompleteBy,
    string? Remarks);
