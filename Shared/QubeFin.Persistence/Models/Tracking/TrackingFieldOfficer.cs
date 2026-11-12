namespace QubeFin.Persistence.Models.Tracking;

public record TrackingFieldOfficer(
    Guid Id,
    string? EmployeeCode,
    string Name,
    string Role,
    string Designation,
    Guid? OrganisationUnitId,
    string? OrganisationUnitName,
    string UnitName,
    IReadOnlyList<OrganizationUnits> OrganisationUnits,
    string? Phone,
    GeoPoint? PresentLocation,
    string? ReportingOfficerName,
    bool CanAssign);


public record OrganizationUnits(Guid id, string name);