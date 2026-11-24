namespace QubeFin.Global.Application.Groups.Models;

// Status of a group (Tbl_Group.CurrentStatus is char(10)).
public static class GroupStatuses
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
}

// Collection day: 1 = Monday ... 7 = Sunday (same list on the screen).
public static class GroupDays
{
    public static readonly string[] Names = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    public static string NameOf(int day) => day >= 1 && day <= 7 ? Names[day - 1] : string.Empty;
}

// The columns of Tbl_Group that the user fills in.
public class GroupRequest
{
    // Set when the group is added from an Approved Group Survey; empty for a manual group.
    public Guid? SurveyId { get; set; }
    // The Credit Officer's designation: the group belongs to it.
    public Guid DesignationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CollectionDay { get; set; }
    public TimeOnly CollectionTime { get; set; }
    public DateOnly? BiweeklyStartDate { get; set; }
    public DateOnly FormationDate { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public Guid AdministrativeUnitId { get; set; }
    public string? PinCode { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? LandMark { get; set; }
}

public class GroupListItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string? AreaName { get; set; }
    public string? CreditOfficerName { get; set; }
    public int CollectionDay { get; set; }
    public string CollectionDayName { get; set; } = string.Empty;
    public TimeOnly CollectionTime { get; set; }
    public DateOnly FormationDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GroupSearchResponse
{
    public List<GroupListItem> Results { get; set; } = new();
    public int TotalRecords { get; set; }
}

public class GroupDetailResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid? SurveyId { get; set; }
    public string? SurveyAssignmentNo { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? DesignationId { get; set; }
    public string? CreditOfficerName { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CollectionDay { get; set; }
    public TimeOnly CollectionTime { get; set; }
    public DateOnly? BiweeklyStartDate { get; set; }
    public DateOnly FormationDate { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public Guid AdministrativeUnitId { get; set; }
    public string? AreaName { get; set; }
    public string? PinCode { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? LandMark { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? VerifiedByName { get; set; }
    public DateTime? VerifiedOn { get; set; }
    public string? VerifiedRemarks { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedOn { get; set; }
    // Server decides what the user may do.
    public bool CanEdit { get; set; }
    public bool CanDecide { get; set; }
}

public class GroupCreditOfficerOption
{
    public Guid DesignationId { get; set; }
    public string Name { get; set; } = string.Empty;
}

// An Approved Group Survey that has no group yet. The area, location and landmark fill the form.
public class GroupSurveyOption
{
    public Guid Id { get; set; }
    public string AssignmentNo { get; set; } = string.Empty;
    public string ProposedArea { get; set; } = string.Empty;
    public Guid AdministrativeUnitId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? LandMark { get; set; }
}

// What the Add Group screen needs: whether the user can add, for which branch, and the choices.
public class GroupAddInfoResponse
{
    public bool CanAdd { get; set; }
    public bool IsHead { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public List<GroupCreditOfficerOption> CreditOfficers { get; set; } = new();
    public List<GroupSurveyOption> Surveys { get; set; } = new();
}

public class GroupDecisionRequest
{
    // Approve or Reject
    public string Decision { get; set; } = string.Empty;
    public string? Comments { get; set; }
}
