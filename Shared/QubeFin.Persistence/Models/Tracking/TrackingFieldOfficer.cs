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
    IReadOnlyList<Guid> OrganisationUnitIds,
    string? Phone,
    decimal? HomeLatitude,
    decimal? HomeLongitude,
    string? ReportingOfficerName,
    bool CanAssign);
