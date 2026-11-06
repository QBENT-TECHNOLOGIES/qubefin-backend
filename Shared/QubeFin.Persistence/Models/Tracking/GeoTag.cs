namespace QubeFin.Persistence.Models.Tracking;

// One geo-tagged (or not yet tagged) place of one kind, for the Geo Tags/POIs page
public record GeoTagRow(
    Guid Id,
    string Kind,
    Guid RefId,
    string Name,
    string? RefCode,
    Guid? OrganisationUnitId,
    string? BranchName,
    string? Address,
    GeoPoint? Location,
    int? RadiusMeters);

public record GeoTagFilter(
    string Kind,
    IReadOnlyList<Guid>? BranchIds,
    Guid? OrganisationUnitId,
    string? SearchText,
    bool OnlyUntagged,
    int? BranchRadiusMeters,
    int? MatchRadiusMeters);
