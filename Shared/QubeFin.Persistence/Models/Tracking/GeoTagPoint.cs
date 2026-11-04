namespace QubeFin.Persistence.Models.Tracking;

public record GeoTagPointRow(
    Guid Id,
    string Kind,
    Guid RefId,
    string Name,
    string RefCode,
    Guid OrganisationUnitId,
    string BranchName,
    string? Address,
    GeoPoint? Location,
    int? TaskLocationMatchInMeters,
    bool Verified,
    Guid? CreditOfficerId,
    string? CreditOfficerName,
    Guid? GroupId,
    string? GroupName,
    int? Dpd,
    decimal? OverdueAmount);

public record GeoTagPointsResult(List<GeoTagPointRow> Points, Dictionary<string, int> UntaggedByKind);
