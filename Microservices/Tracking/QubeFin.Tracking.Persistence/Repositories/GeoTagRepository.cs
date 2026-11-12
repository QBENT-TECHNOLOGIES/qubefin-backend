using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IGeoTagRepository
{
    Task<GeoTagPointsResult> GetPointsAsync(Guid organisationUnitId, IReadOnlyCollection<string> kinds, int? taskLocationMatchInMeters, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, TrackingPlace>> GetPlacesAsync(IEnumerable<Guid> refIds, CancellationToken cancellationToken = default);
    Task<List<GeoTagRow>> GetGeoTagsAsync(GeoTagFilter filter, CancellationToken cancellationToken = default);
    Task<GeoTagTarget?> FindGeoTagAsync(Guid id, Guid? viewerEmployeeId, CancellationToken cancellationToken = default);
    Task UpdateLocationAsync(GeoTagTarget target, GeoPoint location, string? photoKey, Guid userId, DateTime now, CancellationToken cancellationToken = default);
}

public class GeoTagRepository(QubeFinDataContext context) : IGeoTagRepository
{
    private const string GroupCentre = "GroupCentre";
    private const string Member = "Member";
    private const string OdMember = "OdMember";
    private const string Branch = "Branch";
    private const string EmployeeHome = "EmployeeHome";

    // Tracking.USP_GetGeoTagPoints: groups and member homes of one branch for the task picker. There is no verification
    // column yet, so every place counts as verified; a place without a location is only counted in UntaggedByKind.
    public async Task<GeoTagPointsResult> GetPointsAsync(Guid organisationUnitId, IReadOnlyCollection<string> kinds, int? taskLocationMatchInMeters, CancellationToken cancellationToken = default)
    {
        var points = new List<GeoTagPointRow>();
        var untagged = new Dictionary<string, int>();
        var includeMembers = kinds.Contains(Member) || kinds.Contains(OdMember);

        var rows = await context.Set<GeoTagPointResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetGeoTagPoints] @OrganisationUnitId = {organisationUnitId}, @IncludeMembers = {includeMembers}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (kinds.Contains(GroupCentre))
        {
            foreach (var g in rows.Where(r => r.Kind == GroupCentre))
            {
                if (g.Location is null)
                {
                    untagged[GroupCentre] = untagged.GetValueOrDefault(GroupCentre) + 1;
                    continue;
                }
                points.Add(new GeoTagPointRow(
                    g.Id, GroupCentre, g.Id, g.GroupName, g.Code, organisationUnitId, g.BranchName, g.AddressLine1,
                    GeoCoordinates.ToGeoPoint(g.Location), taskLocationMatchInMeters, true,
                    g.CreditOfficerId, g.CreditOfficerName, null, null, null, null, g.TaggedByName, g.TaggedOn));
            }
        }

        if (includeMembers)
        {
            foreach (var m in rows.Where(r => r.Kind != GroupCentre))
            {
                // An overdue member is returned once, as OdMember
                var kind = m.Kind;
                if (!kinds.Contains(kind)) continue;
                if (m.Location is null)
                {
                    untagged[kind] = untagged.GetValueOrDefault(kind) + 1;
                    continue;
                }
                points.Add(new GeoTagPointRow(
                    m.Id, kind, m.Id,
                    string.Join(' ', new[] { m.FirstName, m.MiddleName, m.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))),
                    m.Code, organisationUnitId, m.BranchName,
                    string.Join(", ", new[] { m.AddressLine1, m.AddressLine2 }.Where(a => !string.IsNullOrWhiteSpace(a))) is { Length: > 0 } address ? address : null,
                    GeoCoordinates.ToGeoPoint(m.Location), taskLocationMatchInMeters, true,
                    m.CreditOfficerId, m.CreditOfficerName, m.GroupId, m.GroupName,
                    m.Dpd, m.OverdueAmount, m.TaggedByName, m.TaggedOn));
            }
        }

        return new GeoTagPointsResult(points.OrderBy(p => p.Kind).ThenBy(p => p.Name).ToList(), untagged);
    }

    // Tracking.USP_GetTrackingPlaces: the place behind each id: a group, a member (also found through one of their loans)
    // or a branch, with its current location. Where one id matches more than one place the later kind wins, as before.
    public async Task<Dictionary<Guid, TrackingPlace>> GetPlacesAsync(IEnumerable<Guid> refIds, CancellationToken cancellationToken = default)
    {
        var ids = refIds.Distinct().ToList();
        var places = new Dictionary<Guid, TrackingPlace>();
        if (ids.Count == 0) return places;

        var rows = await context.Set<TrackingPlaceResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetTrackingPlaces] @RefIds = {string.Join(',', ids)}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var p in rows.OrderBy(r => r.Priority))
        {
            if (p.Kind == Branch)
            {
                var point = GeoCoordinates.ToGeoPoint(p.Latitude, p.Longitude);
                places[p.RefId] = new TrackingPlace(p.RefId, Branch, p.BranchId, point is null ? null : GeoCoordinates.ToGeometry(point));
            }
            else
            {
                places[p.RefId] = new TrackingPlace(p.RefId, p.Kind, p.BranchId, p.Location);
            }
        }
        return places;
    }

    // Tracking.USP_GetGeoTags: places of one kind, with and without a location, sorted by name. BranchIds null = every
    // branch (an Admin). Tagged by / at: TaggedBy / TaggedOn, the last user and time the location was set.
    // They are only shown once the place has a location.
    public async Task<List<GeoTagRow>> GetGeoTagsAsync(GeoTagFilter filter, CancellationToken cancellationToken = default)
    {
        var text = string.IsNullOrWhiteSpace(filter.SearchText) ? null : filter.SearchText.Trim();
        var kind = filter.Kind is Branch or GroupCentre or Member or OdMember ? filter.Kind : EmployeeHome;

        var places = await context.Set<GeoTagResult>()
            .FromSqlRaw(
                "EXEC [Tracking].[USP_GetGeoTags] @Kind, @BranchIds, @OrganisationUnitId, @SearchText, @OnlyUntagged, @RefId",
                new SqlParameter("@Kind", SqlDbType.NVarChar, 50) { Value = kind },
                new SqlParameter("@BranchIds", SqlDbType.NVarChar, -1)
                {
                    Value = filter.BranchIds is null ? DBNull.Value : string.Join(',', filter.BranchIds)
                },
                new SqlParameter("@OrganisationUnitId", SqlDbType.UniqueIdentifier) { Value = (object?)filter.OrganisationUnitId ?? DBNull.Value },
                // A branch is searched here, by name or code, as before
                new SqlParameter("@SearchText", SqlDbType.NVarChar, 200) { Value = kind == Branch ? DBNull.Value : (object?)text ?? DBNull.Value },
                new SqlParameter("@OnlyUntagged", SqlDbType.Bit) { Value = filter.OnlyUntagged },
                new SqlParameter("@RefId", SqlDbType.UniqueIdentifier) { Value = (object?)filter.RefId ?? DBNull.Value })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        List<GeoTagRow> rows;
        switch (kind)
        {
            case Branch:
                rows = places
                    .Where(b => text == null || b.Name!.Contains(text, StringComparison.OrdinalIgnoreCase) || b.Code!.Contains(text))
                    .Select(b =>
                    {
                        var location = GeoCoordinates.ToGeoPoint(b.Latitude, b.Longitude);
                        return new GeoTagRow(Branch, b.RefId, b.Name!, b.Code, b.RefId, b.Name, null, location,
                            filter.BranchRadiusMeters, null, location is null ? null : b.TaggedByName, location is null ? null : b.TaggedOn);
                    })
                    .ToList();
                break;
            case GroupCentre:
                rows = places
                    .Select(g => new GeoTagRow(GroupCentre, g.RefId, g.Name!, g.Code, g.BranchId, g.BranchName, g.AddressLine1,
                        GeoCoordinates.ToGeoPoint(g.Location), filter.MatchRadiusMeters, g.LocationPhoto,
                        g.Location is null ? null : g.TaggedByName, g.Location is null ? null : g.TaggedOn))
                    .ToList();
                break;
            case Member:
            case OdMember:
                rows = places
                    .Select(m => new GeoTagRow(filter.Kind, m.RefId,
                        string.Join(' ', new[] { m.FirstName, m.MiddleName, m.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))),
                        m.Code, m.BranchId, m.BranchName,
                        string.Join(", ", new[] { m.AddressLine1, m.AddressLine2 }.Where(a => !string.IsNullOrWhiteSpace(a))) is { Length: > 0 } address ? address : null,
                        GeoCoordinates.ToGeoPoint(m.Location), filter.MatchRadiusMeters, m.LocationPhoto,
                        m.Location is null ? null : m.TaggedByName, m.Location is null ? null : m.TaggedOn))
                    .ToList();
                break;
            default:
                // EmployeeHome: every active employee, placed in the unit of Tbl_Employee.OrganizationUnitId
                rows = places
                    .Select(e => new GeoTagRow(EmployeeHome, e.RefId, e.Name!, e.Code, e.BranchId, e.BranchName, e.AddressLine1,
                        GeoCoordinates.ToGeoPoint(e.Location), filter.MatchRadiusMeters, e.LocationPhoto,
                        e.Location is null ? null : e.TaggedByName, e.Location is null ? null : e.TaggedOn))
                    .ToList();
                break;
        }
        return rows.OrderBy(r => r.Name).ToList();
    }

    // Tracking.USP_FindGeoTag: the place behind a geo-tag id. A group is owned by its mapped designation (DesignationId);
    // a member by its group's.
    public async Task<GeoTagTarget?> FindGeoTagAsync(Guid id, Guid? viewerEmployeeId, CancellationToken cancellationToken = default)
    {
        var target = (await context.Set<GeoTagTargetResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_FindGeoTag] @Id = {id}, @ViewerEmployeeId = {viewerEmployeeId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .FirstOrDefault();
        return target is null
            ? null
            : new GeoTagTarget(target.Kind, id, target.BranchId, target.OwnerDesignationId, target.OwnerPostName, target.OwnerUnitId, target.IsOwnedByViewer);
    }

    // Saves a new location and who set it (TaggedBy / TaggedOn). Branches keep decimal latitude / longitude, the others a
    // geography point. The photo key is stored only when a photo was uploaded.
    public async Task UpdateLocationAsync(GeoTagTarget target, GeoPoint location, string? photoKey, Guid userId, DateTime now, CancellationToken cancellationToken = default)
    {
        switch (target.Kind)
        {
            case Branch:
                var branch = await context.TblOrganizationUnits.FirstAsync(o => o.Id == target.RefId, cancellationToken);
                branch.Latitude = location.Lat;
                branch.Longitude = location.Lng;
                branch.TaggedBy = userId;
                branch.TaggedOn = now;
                break;
            case GroupCentre:
                var group = await context.TblBcgroups.FirstAsync(g => g.Id == target.RefId, cancellationToken);
                group.Location = GeoCoordinates.ToGeometry(location);
                if (photoKey is not null) group.LocationPhoto = photoKey;
                group.TaggedBy = userId;
                group.TaggedOn = now;
                break;
            case Member:
            case OdMember:
                var member = await context.TblBcmembers.FirstAsync(m => m.Id == target.RefId, cancellationToken);
                member.Location = GeoCoordinates.ToGeometry(location);
                if (photoKey is not null) member.LocationPhoto = photoKey;
                member.TaggedBy = userId;
                member.TaggedOn = now;
                break;
            default:
                var employee = await context.TblEmployees.FirstAsync(e => e.Id == target.RefId, cancellationToken);
                employee.PresentLocation = GeoCoordinates.ToGeometry(location);
                if (photoKey is not null) employee.LocationPhoto = photoKey;
                employee.TaggedBy = userId;
                employee.TaggedOn = now;
                break;
        }
    }
}
