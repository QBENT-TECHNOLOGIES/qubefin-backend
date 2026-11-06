namespace QubeFin.Persistence.Models.Tracking;

public record TrackingFieldOfficer(
    Guid Id,
    Guid DesignationId,
    string? EmployeeCode,
    string Name,
    string Role,
    string Designation,
    Guid? OrganisationUnitId,
    string? OrganisationUnitName,
    string UnitName,
    IReadOnlyList<Guid> OrganisationUnitIds,
    string? Phone,
    GeoPoint? PresentLocation,
    string? ReportingOfficerName,
    bool CanAssign);
