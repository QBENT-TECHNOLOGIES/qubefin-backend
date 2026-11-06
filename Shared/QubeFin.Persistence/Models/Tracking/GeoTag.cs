namespace QubeFin.Persistence.Models.Tracking;

// One geo-tagged (or not yet tagged) place of one kind, for the Geo Tags/POIs page
public record GeoTagRow(
    string Kind,
    Guid RefId,
    string Name,
    string? RefCode,
    Guid? BranchId,
    string? BranchName,
    string? Address,
    GeoPoint? Location,
    int? RadiusMeters,
    string? PhotoUrl,
    string? TaggedByName,
    DateTime? TaggedAt);

public record GeoTagFilter(
    string Kind,
    IReadOnlyList<Guid>? BranchIds,
    Guid? OrganisationUnitId,
    string? SearchText,
    bool OnlyUntagged,
    int? BranchRadiusMeters,
    int? MatchRadiusMeters,
    Guid? RefId = null);

// The place behind a geo-tag id, with the designation that owns it (a group's mapped CO; a member uses its group's)
public record GeoTagTarget(
    string Kind,
    Guid RefId,
    Guid? BranchId,
    Guid? OwnerDesignationId,
    string? OwnerPostName,
    Guid? OwnerUnitId,
    bool IsOwnedByViewer);
