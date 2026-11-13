using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Models.Tracking;

public record OdMemberFilter(
    Guid UserId,
    string? SearchText,
    Guid? OrganisationUnitId,
    Guid? CreditOfficerId,
    Guid? GroupId,
    int? DpdMin,
    int? DpdMax,
    bool OnlyUnassigned,
    string? SortOn,
    bool SortDescending,
    int PageIndex,
    int PageSize);

public record OdMemberSearchResult(List<OdMemberSearchRowResult> Members, int TotalRecords, decimal TotalOverdueAmount);

// One row of Tracking.USP_SearchOdMembers. FollowUpJson is the latest OD follow-up task (one object),
// PendingWithJson the pending ones (an array); both camelCase.
public class OdMemberSearchRowResult
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
    public string LoanAccountNo { get; set; } = null!;
    public string MemberName { get; set; } = null!;
    public string MemberCode { get; set; } = null!;
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string GroupCode { get; set; } = null!;
    public Guid? CreditOfficerId { get; set; }
    public string? CreditOfficerName { get; set; }
    public Guid OrganisationUnitId { get; set; }
    public string BranchName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal? OverdueAmount { get; set; }
    public int? Dpd { get; set; }
    public DateOnly? LastPaymentDate { get; set; }
    public Geometry? Location { get; set; }
    public string? FollowUpJson { get; set; }
    public string? PendingWithJson { get; set; }
}

// Open jobs for the "Add to existing job" list (Tracking.USP_GetOpenJobs)
public record OpenJobFilter(Guid UserId, Guid? OrganisationUnitId, Guid? AssigneeId, string? SearchText);
