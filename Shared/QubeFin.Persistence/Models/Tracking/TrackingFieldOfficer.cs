namespace QubeFin.Persistence.Models.Tracking;

public record TrackingFieldOfficer(
    Guid Id,
    string? EmployeeCode,
    string Name,
    string Role,
    string Designation,
    Guid? OrganizationUnitId,
    string? OrganizationUnitName,
    string UnitName,
    IReadOnlyList<OrganizationUnits> OrganizationUnits,
    string? Phone,
    GeoPoint? PresentLocation,
    string? ReportingOfficerName,
    bool CanAssign);


public record OrganizationUnits(Guid id, string name);