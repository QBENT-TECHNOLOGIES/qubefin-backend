using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface ITrackingScopeRepository
{
    Task<TrackingViewer?> GetViewerAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasAssignableEmployeesAsync(TrackingViewer viewer, CancellationToken cancellationToken = default);
}

public class TrackingScopeRepository(QubeFinDataContext context) : ITrackingScopeRepository
{
    private const string BranchUnitType = "Branch";
    private const string AllBranches = "All branches";

    public async Task<TrackingViewer?> GetViewerAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await context.TblUsers
            .Where(u => u.Id == userId && u.IsActive)
            .Select(u => new { u.Id, u.UserName, u.IsSuperAdmin, u.EmployeeId, EmployeeName = u.Employee != null ? u.Employee.FullName : null })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) return null;

        var units = await context.TblOrganizationUnits
            .Select(o => new UnitNode(o.Id, o.ParentId, o.Name, o.OrganizationUnitType.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (user.IsSuperAdmin)
        {
            return new TrackingViewer(
                user.Id,
                user.EmployeeId,
                user.EmployeeName ?? user.UserName,
                TrackingRole.Admin,
                null,
                AllBranches,
                units.Select(u => u.Id).ToList(),
                BranchIdsOf(units).ToList());
        }

        if (user.EmployeeId is null) return null;

        var position = await context.TblEmployeeDesignations
            .Where(ed => ed.EmployeeId == user.EmployeeId && ed.EffectiveTo == null)
            .OrderByDescending(ed => ed.EffectiveFrom)
            .Select(ed => new { PostName = ed.Designation.Post.Name, ed.Designation.OrganizationUnitId, UnitName = ed.Designation.OrganizationUnit.Name })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (position is null || !TrackingRole.IsTrackingPost(position.PostName)) return null;

        var scope = DescendantsAndSelf(units, position.OrganizationUnitId);
        return new TrackingViewer(
            user.Id,
            user.EmployeeId,
            user.EmployeeName ?? user.UserName,
            position.PostName,
            position.OrganizationUnitId,
            position.UnitName,
            scope.Select(u => u.Id).ToList(),
            BranchIdsOf(scope).ToList());
    }

    public async Task<bool> HasAssignableEmployeesAsync(TrackingViewer viewer, CancellationToken cancellationToken = default)
    {
        var postNamesBelow = TrackingRole.PostNamesBelow(viewer.Role).ToList();
        if (postNamesBelow.Count == 0) return false;

        return await context.TblEmployeeDesignations
            .AnyAsync(ed => ed.EffectiveTo == null
                && ed.Employee.IsActive
                && ed.EmployeeId != viewer.EmployeeId
                && postNamesBelow.Contains(ed.Designation.Post.Name)
                && viewer.UnitIdsInScope.Contains(ed.Designation.OrganizationUnitId),
                cancellationToken);
    }

    private sealed record UnitNode(Guid Id, Guid? ParentId, string Name, string TypeName);

    private static IEnumerable<Guid> BranchIdsOf(IEnumerable<UnitNode> units) =>
        units.Where(u => u.TypeName == BranchUnitType).OrderBy(u => u.Name).Select(u => u.Id);

    private static List<UnitNode> DescendantsAndSelf(List<UnitNode> units, Guid rootId)
    {
        var children = units.Where(u => u.ParentId.HasValue).ToLookup(u => u.ParentId!.Value);
        var result = units.Where(u => u.Id == rootId).ToList();
        var visited = new HashSet<Guid> { rootId };
        for (var i = 0; i < result.Count; i++)
        {
            foreach (var child in children[result[i].Id])
            {
                if (visited.Add(child.Id)) result.Add(child);
            }
        }
        return result;
    }
}
