using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface IGeoTagRepository
{
    Task<GeoTagPointsResult> GetPointsAsync(Guid organisationUnitId, IReadOnlyCollection<string> kinds, int? taskLocationMatchInMeters, CancellationToken cancellationToken = default);
}

public class GeoTagRepository(QubeFinDataContext context) : IGeoTagRepository
{
    private const string GroupCentre = "GroupCentre";
    private const string Member = "Member";
    private const string OdMember = "OdMember";

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
