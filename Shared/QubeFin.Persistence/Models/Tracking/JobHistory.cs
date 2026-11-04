namespace QubeFin.Persistence.Models.Tracking;

public record JobHistoryRow(
    Guid Id,
    Guid JobId,
    string Action,
    string PerformedByName,
    DateTime? PerformedOn,
    string? FromAssigneeName,
    string? ToAssigneeName,
    IReadOnlyList<string> TaskNos,
    Guid? RelatedJobId,
    string? RelatedJobCode,
    string? Remarks);
