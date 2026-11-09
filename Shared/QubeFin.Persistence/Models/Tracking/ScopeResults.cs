namespace QubeFin.Persistence.Models.Tracking;

// One row of Global.Fn_GetUserWiseBranches
public class UserWiseBranchResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? AreaName { get; set; }
    public string? DivisionName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

// One row of Global.USP_GetFieldOfficers
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
    public string? OrganisationUnitIds { get; set; }
    public string? Phone { get; set; }
    public double? PresentLatitude { get; set; }
    public double? PresentLongitude { get; set; }
    public string? ReportingOfficerName { get; set; }
    public bool CanAssign { get; set; }
}
