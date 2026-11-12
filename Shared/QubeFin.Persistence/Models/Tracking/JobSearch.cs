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

// One row of Tracking.USP_SearchJobs; BranchesJson is [{ "BranchId", "Name" }] sorted by name, TypesCsv the distinct task types
public class JobSearchRowResult
{
    public Guid Id { get; set; }
    public string JobCode { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public Guid? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public string AssigneeRole { get; set; } = null!;
    public string UnitName { get; set; } = null!;
    public string? BranchesJson { get; set; }
    public string CreatedByName { get; set; } = null!;
    public DateTime? CreatedOn { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int TaskCount { get; set; }
    public int DoneCount { get; set; }
    public int OpenCount { get; set; }
    public int SelfAddedCount { get; set; }
    public string? TypesCsv { get; set; }
    public Guid? TransferJobRefId { get; set; }
    public string? TransferJobRefCode { get; set; }
}
