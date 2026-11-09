namespace QubeFin.Persistence.Models.Tracking;

// One row of Global.USP_GetBCGroupList
public class BcGroupListResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? AddressLine1 { get; set; }
    public Guid? AdministrativeUnitId { get; set; }
    public string? AdministrativeUnitName { get; set; }
    public string? PinCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationPhoto { get; set; }
    public Guid OrganizationUnitId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid DesignationId { get; set; }
    public Guid? CoEmployeeId { get; set; }
    public string? CoName { get; set; }
    public DateOnly? FormationDate { get; set; }
    public string? ContactNo { get; set; }
    public string? CollectionDay { get; set; }
    public TimeOnly? CollectionTime { get; set; }
    public DateOnly? BiWeeklyStartDate { get; set; }
    public bool IsMonthlyGroup { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public Guid? TaggedBy { get; set; }
    public string? TaggedByName { get; set; }
    public DateTime? TaggedOn { get; set; }
    public int TotalRecords { get; set; }
}

// One row of Global.USP_GetBCMemberList
public class BcMemberListResult
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? FatherName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Religion { get; set; }
    public string? Caste { get; set; }
    public string? Qualification { get; set; }
    public string? AadharNo { get; set; }
    public string? Pan { get; set; }
    public string? VoterNo { get; set; }
    public string? MobileNo { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? PinCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LocationPhoto { get; set; }
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public Guid OrganizationUnitId { get; set; }
    public string BranchName { get; set; } = null!;
    public Guid? CoEmployeeId { get; set; }
    public string? CoName { get; set; }
    public bool IsOverdueMember { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public Guid? TaggedBy { get; set; }
    public string? TaggedByName { get; set; }
    public DateTime? TaggedOn { get; set; }
    public int TotalRecords { get; set; }
}

// Filters for both lists; the procedures work out whose groups / members the user may see from UserId
public record BcListFilter(
    Guid UserId,
    Guid? OrganisationUnitId,
    Guid? CoEmployeeId,
    Guid? GroupId,
    string? SearchText,
    string? SortOn,
    string? SortDirection,
    int PageIndex,
    int PageSize);
