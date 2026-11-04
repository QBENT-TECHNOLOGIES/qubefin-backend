namespace QubeFin.Persistence.Models.Tracking;

public record TrackingBranch(
    Guid Id,
    string Name,
    string Code,
    string? AreaName,
    string? DivisionName,
    GeoPoint? Location,
    int? BranchOperatingRadiusInKm);
