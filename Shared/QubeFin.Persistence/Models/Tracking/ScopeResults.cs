namespace QubeFin.Persistence.Models.Tracking;

// One branch of Global.Fn_GetUserWiseBranches, with its code
public class UserWiseBranchItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}

// One row of Global.USP_GetBranchWiseCreditOfficers
public class BranchCreditOfficerItem
{
    public Guid EmployeeId { get; set; }
    public string? EmployeeCode { get; set; }
    public string Name { get; set; } = null!;
}

// One row of Global.USP_GetFieldOfficers; OrganisationUnitsJson is [{ "id", "name" }]
public class FieldOfficerResult
{
    public Guid Id { get; set; }
    public string? EmployeeCode { get; set; }
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public Guid? OrganisationUnitId { get; set; }
    public string? OrganisationUnitName { get; set; }
    public string UnitName { get; set; } = null!;
    public string? OrganisationUnitsJson { get; set; }
    public string? Phone { get; set; }
    public double? PresentLatitude { get; set; }
    public double? PresentLongitude { get; set; }
    public string? ReportingOfficerName { get; set; }
    public bool CanAssign { get; set; }
}
