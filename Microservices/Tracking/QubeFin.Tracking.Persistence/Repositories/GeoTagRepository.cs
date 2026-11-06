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
    Task UpdateLocationAsync(GeoTagTarget target, GeoPoint location, Guid userId, DateTime now, CancellationToken cancellationToken = default);
}

public class GeoTagRepository(QubeFinDataContext context) : IGeoTagRepository
{
    private const string GroupCentre = "GroupCentre";
    private const string Member = "Member";
    private const string OdMember = "OdMember";
    private const string Branch = "Branch";
    private const string EmployeeHome = "EmployeeHome";

    // Groups and member homes of one branch for the task picker. There is no verification column yet, so every
    // place counts as verified; a place without a location is only counted in UntaggedByKind.
    public async Task<GeoTagPointsResult> GetPointsAsync(Guid organisationUnitId, IReadOnlyCollection<string> kinds, int? taskLocationMatchInMeters, CancellationToken cancellationToken = default)
    {
        var points = new List<GeoTagPointRow>();
        var untagged = new Dictionary<string, int>();
        var branchName = await context.TblOrganizationUnits
            .Where(o => o.Id == organisationUnitId)
            .Select(o => o.Name)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var groups = await context.TblBcgroups
            .Where(g => g.OrganizationUnitId == organisationUnitId)
            .Select(g => new { g.Id, g.Code, g.Name, g.AddressLine1, g.DesignationId, g.Location })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var creditOfficers = await DesignationHoldersAsync(groups.Select(g => g.DesignationId), cancellationToken);

        if (kinds.Contains(GroupCentre))
        {
            foreach (var g in groups)
            {
                if (g.Location is null)
                {
                    untagged[GroupCentre] = untagged.GetValueOrDefault(GroupCentre) + 1;
                    continue;
                }
                var creditOfficer = creditOfficers.GetValueOrDefault(g.DesignationId);
                points.Add(new GeoTagPointRow(
                    g.Id, GroupCentre, g.Id, g.Name, g.Code, organisationUnitId, branchName, g.AddressLine1,
                    GeoCoordinates.ToGeoPoint(g.Location), taskLocationMatchInMeters, true,
                    creditOfficer?.EmployeeId, creditOfficer?.Name, null, null, null, null));
            }
        }

        if (kinds.Contains(Member) || kinds.Contains(OdMember))
        {
            var groupIds = groups.Select(g => g.Id).ToList();
            var members = await context.TblBcmembers
                .Where(m => groupIds.Contains(m.BcgroupId))
                .Select(m => new { m.Id, m.Code, m.FirstName, m.MiddleName, m.LastName, m.AddressLine1, m.AddressLine2, m.BcgroupId, m.IsOverdueMember, m.Location })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // The member's current DPD is the highest DPD of their open loans; the overdue amount is the total
            var memberIds = members.Where(m => m.IsOverdueMember).Select(m => m.Id).ToList();
            var dues = await context.TblBcloans
                .Where(l => !l.IsClosed && memberIds.Contains(l.BcmemberId))
                .GroupBy(l => l.BcmemberId)
                .Select(g => new { MemberId = g.Key, Dpd = g.Max(l => l.Dpddays), OverdueAmount = g.Sum(l => l.OverdueAmount) })
                .ToDictionaryAsync(d => d.MemberId, cancellationToken);
            var groupsById = groups.ToDictionary(g => g.Id);

            foreach (var m in members)
            {
                // An overdue member is returned once, as OdMember
                var kind = m.IsOverdueMember ? OdMember : Member;
                if (!kinds.Contains(kind)) continue;
                if (m.Location is null)
                {
                    untagged[kind] = untagged.GetValueOrDefault(kind) + 1;
                    continue;
                }
                var group = groupsById[m.BcgroupId];
                var creditOfficer = creditOfficers.GetValueOrDefault(group.DesignationId);
                var due = m.IsOverdueMember ? dues.GetValueOrDefault(m.Id) : null;
                points.Add(new GeoTagPointRow(
                    m.Id, kind, m.Id,
                    string.Join(' ', new[] { m.FirstName, m.MiddleName, m.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))),
                    m.Code, organisationUnitId, branchName,
                    string.Join(", ", new[] { m.AddressLine1, m.AddressLine2 }.Where(a => !string.IsNullOrWhiteSpace(a))) is { Length: > 0 } address ? address : null,
                    GeoCoordinates.ToGeoPoint(m.Location), taskLocationMatchInMeters, true,
                    creditOfficer?.EmployeeId, creditOfficer?.Name, group.Id, group.Name,
                    due?.Dpd, due?.OverdueAmount));
            }
        }

        return new GeoTagPointsResult(points.OrderBy(p => p.Kind).ThenBy(p => p.Name).ToList(), untagged);
    }

    // The place behind each id: a group, a member (also found through one of their loans) or a branch, with its current location
    public async Task<Dictionary<Guid, TrackingPlace>> GetPlacesAsync(IEnumerable<Guid> refIds, CancellationToken cancellationToken = default)
    {
        var ids = refIds.Distinct().ToList();
        var places = new Dictionary<Guid, TrackingPlace>();
        if (ids.Count == 0) return places;

        foreach (var g in await context.TblBcgroups.Where(g => ids.Contains(g.Id)).Select(g => new { g.Id, g.OrganizationUnitId, g.Location }).AsNoTracking().ToListAsync(cancellationToken))
        {
            places[g.Id] = new TrackingPlace(g.Id, GroupCentre, g.OrganizationUnitId, g.Location);
        }

        var members = await context.TblBcmembers
            .Where(m => ids.Contains(m.Id) || m.TblBcloans.Any(l => ids.Contains(l.Id)))
            .Select(m => new { m.Id, m.IsOverdueMember, m.Bcgroup.OrganizationUnitId, m.Location, LoanIds = m.TblBcloans.Where(l => ids.Contains(l.Id)).Select(l => l.Id).ToList() })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        foreach (var m in members)
        {
            var kind = m.IsOverdueMember ? OdMember : Member;
            if (ids.Contains(m.Id)) places[m.Id] = new TrackingPlace(m.Id, kind, m.OrganizationUnitId, m.Location);
            foreach (var loanId in m.LoanIds) places[loanId] = new TrackingPlace(loanId, OdMember, m.OrganizationUnitId, m.Location);
        }

        foreach (var b in await context.TblOrganizationUnits.Where(o => ids.Contains(o.Id)).Select(o => new { o.Id, o.Latitude, o.Longitude }).AsNoTracking().ToListAsync(cancellationToken))
        {
            var point = GeoCoordinates.ToGeoPoint(b.Latitude, b.Longitude);
            places[b.Id] = new TrackingPlace(b.Id, "Branch", b.Id, point is null ? null : GeoCoordinates.ToGeometry(point));
        }
        return places;
    }

    // Places of one kind, with and without a location, sorted by name. BranchIds null = every branch (an Admin).
    // Tagged by / at: groups and members use CreatedBy / CreatedDate; branches and employees LastModifiedBy / LastModifiedOn.
    // They are only shown once the place has a location.
    public async Task<List<GeoTagRow>> GetGeoTagsAsync(GeoTagFilter filter, CancellationToken cancellationToken = default)
    {
        var text = string.IsNullOrWhiteSpace(filter.SearchText) ? null : filter.SearchText.Trim();
        var branchIds = filter.BranchIds;
        var unitId = filter.OrganisationUnitId;
        var refId = filter.RefId;

        List<GeoTagRow> rows;
        switch (filter.Kind)
        {
            case Branch:
            {
                var branches = await context.TblOrganizationUnits
                    .Where(o => o.OrganizationUnitType.Name == Branch)
                    .Where(o => refId == null || o.Id == refId)
                    .Where(o => branchIds == null || branchIds.Contains(o.Id))
                    .Where(o => unitId == null || o.Id == unitId)
                    .Where(o => !filter.OnlyUntagged || o.Latitude == null || o.Longitude == null)
                    .Select(o => new
                    {
                        o.Id, o.Name, o.CodeVal, o.Latitude, o.Longitude, o.LastModifiedOn,
                        TaggedByName = o.LastModifiedByNavigation != null
                            ? (o.LastModifiedByNavigation.Employee != null ? o.LastModifiedByNavigation.Employee.FullName : o.LastModifiedByNavigation.UserName)
                            : null
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                rows = branches
                    .Where(b => text == null || b.Name.Contains(text, StringComparison.OrdinalIgnoreCase) || b.CodeVal.ToString().Contains(text))
                    .Select(b =>
                    {
                        var location = GeoCoordinates.ToGeoPoint(b.Latitude, b.Longitude);
                        return new GeoTagRow(Branch, b.Id, b.Name, b.CodeVal.ToString(), b.Id, b.Name, null, location,
                            filter.BranchRadiusMeters, null, location is null ? null : b.TaggedByName, location is null ? null : b.LastModifiedOn);
                    })
                    .ToList();
                break;
            }
            case GroupCentre:
            {
                var groups = await context.TblBcgroups
                    .Where(g => refId == null || g.Id == refId)
                    .Where(g => branchIds == null || branchIds.Contains(g.OrganizationUnitId))
                    .Where(g => unitId == null || g.OrganizationUnitId == unitId)
                    .Where(g => !filter.OnlyUntagged || g.Location == null)
                    .Where(g => text == null || g.Name.Contains(text) || g.Code.Contains(text))
                    .Select(g => new
                    {
                        g.Id, g.Name, g.Code, g.OrganizationUnitId, BranchName = g.OrganizationUnit.Name, g.AddressLine1, g.Location, g.LocationPhoto, g.CreatedDate,
                        TaggedByName = g.CreatedByNavigation != null
                            ? (g.CreatedByNavigation.Employee != null ? g.CreatedByNavigation.Employee.FullName : g.CreatedByNavigation.UserName)
                            : null
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                rows = groups
                    .Select(g => new GeoTagRow(GroupCentre, g.Id, g.Name, g.Code, g.OrganizationUnitId, g.BranchName, g.AddressLine1,
                        GeoCoordinates.ToGeoPoint(g.Location), filter.MatchRadiusMeters, g.LocationPhoto,
                        g.Location is null ? null : g.TaggedByName, g.Location is null ? null : g.CreatedDate))
                    .ToList();
                break;
            }
            case Member:
            case OdMember:
            {
                // Member lists every member; OdMember only the ones overdue now
                var overdueOnly = filter.Kind == OdMember;
                var members = await context.TblBcmembers
                    .Where(m => refId == null || m.Id == refId)
                    .Where(m => !overdueOnly || m.IsOverdueMember)
                    .Where(m => branchIds == null || branchIds.Contains(m.Bcgroup.OrganizationUnitId))
                    .Where(m => unitId == null || m.Bcgroup.OrganizationUnitId == unitId)
                    .Where(m => !filter.OnlyUntagged || m.Location == null)
                    .Where(m => text == null || m.FirstName.Contains(text) || (m.MiddleName != null && m.MiddleName.Contains(text))
                        || m.LastName.Contains(text) || m.Code.Contains(text))
                    .Select(m => new
                    {
                        m.Id, m.Code, m.FirstName, m.MiddleName, m.LastName, m.AddressLine1, m.AddressLine2, m.Location, m.LocationPhoto, m.CreatedDate,
                        m.Bcgroup.OrganizationUnitId, BranchName = m.Bcgroup.OrganizationUnit.Name,
                        TaggedByName = m.CreatedByNavigation != null
                            ? (m.CreatedByNavigation.Employee != null ? m.CreatedByNavigation.Employee.FullName : m.CreatedByNavigation.UserName)
                            : null
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                rows = members
                    .Select(m => new GeoTagRow(filter.Kind, m.Id,
                        string.Join(' ', new[] { m.FirstName, m.MiddleName, m.LastName }.Where(n => !string.IsNullOrWhiteSpace(n))),
                        m.Code, m.OrganizationUnitId, m.BranchName,
                        string.Join(", ", new[] { m.AddressLine1, m.AddressLine2 }.Where(a => !string.IsNullOrWhiteSpace(a))) is { Length: > 0 } address ? address : null,
                        GeoCoordinates.ToGeoPoint(m.Location), filter.MatchRadiusMeters, m.LocationPhoto,
                        m.Location is null ? null : m.TaggedByName, m.Location is null ? null : m.CreatedDate))
                    .ToList();
                break;
            }
            default:
            {
                // EmployeeHome: every active employee, placed in the unit of Tbl_Employee.OrganizationUnitId
                var employees = await context.TblEmployees
                    .Where(e => refId == null || e.Id == refId)
                    .Where(e => e.IsActive)
                    .Where(e => branchIds == null || (e.OrganizationUnitId != null && branchIds.Contains(e.OrganizationUnitId.Value)))
                    .Where(e => unitId == null || e.OrganizationUnitId == unitId)
                    .Where(e => !filter.OnlyUntagged || e.PresentLocation == null)
                    .Where(e => text == null || e.FullName.Contains(text) || (e.Code != null && e.Code.Contains(text)))
                    .Select(e => new
                    {
                        e.Id, e.FullName, e.Code, e.OrganizationUnitId,
                        BranchName = e.OrganizationUnit != null ? e.OrganizationUnit.Name : null,
                        e.PresentAddress, e.PresentLocation, e.LocationPhoto, e.LastModifiedOn,
                        TaggedByName = context.TblUsers
                            .Where(u => u.Id == e.LastModifiedBy)
                            .Select(u => u.Employee != null ? u.Employee.FullName : u.UserName)
                            .FirstOrDefault()
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                rows = employees
                    .Select(e => new GeoTagRow(EmployeeHome, e.Id, e.FullName, e.Code, e.OrganizationUnitId, e.BranchName, e.PresentAddress,
                        GeoCoordinates.ToGeoPoint(e.PresentLocation), filter.MatchRadiusMeters, e.LocationPhoto,
                        e.PresentLocation is null ? null : e.TaggedByName, e.PresentLocation is null ? null : e.LastModifiedOn))
                    .ToList();
                break;
            }
        }
        return rows.OrderBy(r => r.Name).ToList();
    }

    // Finds the place behind a geo-tag id. A group is owned by its mapped designation (DesignationId); a member by its group's.
    public async Task<GeoTagTarget?> FindGeoTagAsync(Guid id, Guid? viewerEmployeeId, CancellationToken cancellationToken = default)
    {
        var isBranch = await context.TblOrganizationUnits.AnyAsync(o => o.Id == id && o.OrganizationUnitType.Name == Branch, cancellationToken);
        if (isBranch) return new GeoTagTarget(Branch, id, id, null, null, null, false);

        var owner = await context.TblBcgroups
            .Where(g => g.Id == id)
            .Select(g => new { Kind = GroupCentre, g.OrganizationUnitId, g.DesignationId, PostName = g.Designation.Post.Name, UnitId = g.Designation.OrganizationUnitId })
            .FirstOrDefaultAsync(cancellationToken)
            ?? await context.TblBcmembers
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    Kind = m.IsOverdueMember ? OdMember : Member,
                    m.Bcgroup.OrganizationUnitId,
                    m.Bcgroup.DesignationId,
                    PostName = m.Bcgroup.Designation.Post.Name,
                    UnitId = m.Bcgroup.Designation.OrganizationUnitId
                })
                .FirstOrDefaultAsync(cancellationToken);
        if (owner is not null)
        {
            var isOwnedByViewer = viewerEmployeeId.HasValue && await context.TblEmployeeDesignations
                .AnyAsync(ed => ed.EmployeeId == viewerEmployeeId && ed.DesignationId == owner.DesignationId && ed.EffectiveTo == null, cancellationToken);
            return new GeoTagTarget(owner.Kind, id, owner.OrganizationUnitId, owner.DesignationId, owner.PostName, owner.UnitId, isOwnedByViewer);
        }

        var employeeUnitId = await context.TblEmployees
            .Where(e => e.Id == id && e.IsActive)
            .Select(e => new { e.OrganizationUnitId })
            .FirstOrDefaultAsync(cancellationToken);
        return employeeUnitId is null ? null : new GeoTagTarget(EmployeeHome, id, employeeUnitId.OrganizationUnitId, null, null, null, false);
    }

    // Saves a new location: branches keep decimal latitude / longitude, the others a geography point
    public async Task UpdateLocationAsync(GeoTagTarget target, GeoPoint location, Guid userId, DateTime now, CancellationToken cancellationToken = default)
    {
        switch (target.Kind)
        {
            case Branch:
                var branch = await context.TblOrganizationUnits.FirstAsync(o => o.Id == target.RefId, cancellationToken);
                branch.Latitude = location.Lat;
                branch.Longitude = location.Lng;
                branch.LastModifiedBy = userId;
                branch.LastModifiedOn = now;
                break;
            case GroupCentre:
                var group = await context.TblBcgroups.FirstAsync(g => g.Id == target.RefId, cancellationToken);
                group.Location = GeoCoordinates.ToGeometry(location);
                break;
            case Member:
            case OdMember:
                var member = await context.TblBcmembers.FirstAsync(m => m.Id == target.RefId, cancellationToken);
                member.Location = GeoCoordinates.ToGeometry(location);
                break;
            default:
                var employee = await context.TblEmployees.FirstAsync(e => e.Id == target.RefId, cancellationToken);
                employee.PresentLocation = GeoCoordinates.ToGeometry(location);
                employee.LastModifiedBy = userId;
                employee.LastModifiedOn = now;
                break;
        }
    }

    private sealed record DesignationHolder(Guid EmployeeId, string Name);

    // The employee currently holding each designation (latest if more than one)
    private async Task<Dictionary<Guid, DesignationHolder>> DesignationHoldersAsync(IEnumerable<Guid> designationIds, CancellationToken cancellationToken)
    {
        var ids = designationIds.Distinct().ToList();
        return (await context.TblEmployeeDesignations
            .Where(ed => ids.Contains(ed.DesignationId) && ed.EffectiveTo == null)
            .Select(ed => new { ed.DesignationId, ed.EmployeeId, ed.Employee.FullName, ed.EffectiveFrom })
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .GroupBy(ed => ed.DesignationId)
            .ToDictionary(g => g.Key, g =>
            {
                var latest = g.OrderByDescending(ed => ed.EffectiveFrom).First();
                return new DesignationHolder(latest.EmployeeId, latest.FullName);
            });
    }
}
