namespace QubeFin.Persistence.Models.Tracking;

public record JobSearchFilter(
    string? SearchText,
    string? Status,
    bool OpenOnly,
    Guid? OrganisationUnitId,
    Guid? AssigneeId,
    DateOnly? DueFrom,
    DateOnly? DueTo,
    int? DpdMin,
    int? DpdMax,
    int PageIndex,
    int PageSize,
    string? SortOn,
    bool SortDescending);

public record JobSummaryRow(
    Guid Id,
    string JobCode,
    string JobTitle,
    string Status,
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
    IReadOnlyList<string> Types,
    Guid? TransferJobRefId,
    string? TransferJobRefCode);

public record JobStatusCounts(int Total, int Assigned, int InProgress, int Delayed, int Completed);

public record JobSearchResult(List<JobSummaryRow> Jobs, int TotalRecords, JobStatusCounts Summary);
