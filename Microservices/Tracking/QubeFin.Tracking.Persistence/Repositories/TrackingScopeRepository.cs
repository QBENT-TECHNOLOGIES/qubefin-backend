using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface ITrackingScopeRepository
{
    Task<TrackingViewer?> GetViewerAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<TrackingBranch>> GetBranchesAsync(IReadOnlyList<Guid> branchIds, CancellationToken cancellationToken = default);
    Task<List<UserWiseBranchItem>> GetUserWiseBranchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<BranchCreditOfficerItem>> GetBranchWiseCreditOfficersAsync(Guid userId, Guid organisationUnitId, CancellationToken cancellationToken = default);
    Task<TblTrackingSetting?> GetTrackingSettingAsync(CancellationToken cancellationToken = default);
    Task<List<TrackingFieldOfficer>> GetFieldOfficersAsync(TrackingViewer viewer, Guid? organizationUnitId, CancellationToken cancellationToken = default);
    Task<TrackingOfficer?> GetOfficerAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, BranchCode>> GetBranchCodesAsync(IEnumerable<Guid> branchIds, CancellationToken cancellationToken = default);
    Task<DateOnly?> GetFinancialYearStartAsync(DateOnly day, CancellationToken cancellationToken = default);
}

public class TrackingScopeRepository(QubeFinDataContext context) : ITrackingScopeRepository
{
    private const string BranchUnitType = "Branch";
    private const string AreaUnitType = "Area";
    private const string DivisionUnitType = "Division";
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

    public async Task<List<TrackingBranch>> GetBranchesAsync(IReadOnlyList<Guid> branchIds, CancellationToken cancellationToken = default)
    {
        var units = await context.TblOrganizationUnits
            .Select(o => new UnitNode(o.Id, o.ParentId, o.Name, o.OrganizationUnitType.Name))
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var branches = await context.TblOrganizationUnits
            .Where(o => branchIds.Contains(o.Id))
            .Select(o => new { o.Id, o.Name, o.CodeVal, o.ParentId, o.Latitude, o.Longitude })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var setting = await GetTrackingSettingAsync(cancellationToken);

        return branches
            .OrderBy(b => b.Name)
            .Select(b => new TrackingBranch(
                b.Id,
                b.Name,
                b.CodeVal.ToString(),
                AncestorOfType(units, b.ParentId, AreaUnitType)?.Name,
                AncestorOfType(units, b.ParentId, DivisionUnitType)?.Name,
                GeoCoordinates.ToGeoPoint(b.Latitude, b.Longitude),
                setting?.BranchOperatingRadiusInKm))
            .ToList();
    }

    // The branches of Global.Fn_GetUserWiseBranches (BM one branch, AM the area's, DM the division's, super admin all),
    // with their code, sorted by name
    public async Task<List<UserWiseBranchItem>> GetUserWiseBranchesAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.Set<UserWiseBranchItem>()
            .FromSqlInterpolated($@"SELECT DISTINCT B.Id, B.Name, CAST(OU.CodeVal AS NVARCHAR(20)) AS Code
                FROM [Global].[Fn_GetUserWiseBranches]({userId}) B
                INNER JOIN [Global].[Tbl_OrganizationUnit] OU ON OU.Id = B.Id")
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

    // Global.USP_GetBranchWiseCreditOfficers: Credit Officers of one branch (a Credit Officer gets only themself)
    public async Task<List<BranchCreditOfficerItem>> GetBranchWiseCreditOfficersAsync(Guid userId, Guid organisationUnitId, CancellationToken cancellationToken = default) =>
        await context.Set<BranchCreditOfficerItem>()
            .FromSqlInterpolated($"EXEC [Global].[USP_GetBranchWiseCreditOfficers] @UserId = {userId}, @OrganisationUnitId = {organisationUnitId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    // The active Tbl_TrackingSetting row (latest if more than one is active)
    public async Task<TblTrackingSetting?> GetTrackingSettingAsync(CancellationToken cancellationToken = default) =>
        await context.TblTrackingSettings
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.ModifiedOn ?? s.CreatedOn)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<TrackingFieldOfficer>> GetFieldOfficersAsync(TrackingViewer viewer, Guid? organizationUnitId, CancellationToken cancellationToken = default)
    {
        var units = await context.TblOrganizationUnits
            .Select(o => new UnitNode(o.Id, o.ParentId, o.Name, o.OrganizationUnitType.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var trackingPosts = TrackingRole.PostNamesBelow(TrackingRole.Admin).ToList();
        var positions = (await context.TblEmployeeDesignations
            .Where(ed => ed.EffectiveTo == null
                && ed.Employee.IsActive
                && ed.EmployeeId != viewer.EmployeeId
                && trackingPosts.Contains(ed.Designation.Post.Name)
                && viewer.UnitIdsInScope.Contains(ed.Designation.OrganizationUnitId))
            .Select(ed => new
            {
                ed.EmployeeId,
                ed.EffectiveFrom,
                ed.Employee.Code,
                ed.Employee.FullName,
                PostName = ed.Designation.Post.Name,
                DesignationName = ed.Designation.Name,
                DesignationUnitId = ed.Designation.OrganizationUnitId,
                DesignationUnitName = ed.Designation.OrganizationUnit.Name,
                ed.Employee.OrganizationUnitId,
                OrganizationUnitName = ed.Employee.OrganizationUnit != null ? ed.Employee.OrganizationUnit.Name : null,
                ed.Employee.MobileNo,
                ed.Employee.PresentLocation,
                ReportingOfficerName = ed.Employee.Supervisor != null ? ed.Employee.Supervisor.FullName : null
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .GroupBy(p => p.EmployeeId)
            .Select(g => g.OrderByDescending(p => p.EffectiveFrom).First());

        var officers = new List<TrackingFieldOfficer>();
        foreach (var p in positions)
        {
            var isBelow = TrackingRole.Rank(p.PostName) < TrackingRole.Rank(viewer.Role);
            // A Branch Manager oversees the Assistant Branch Manager of their own branch but cannot give them work
            var isOwnBranchAbm = viewer.Role == TrackingRole.BranchManager
                && p.PostName == TrackingRole.AssistantBranchManager
                && p.DesignationUnitId == viewer.UnitId;
            if (!isBelow && !isOwnBranchAbm) continue;

            var officerBranchIds = BranchIdsOf(DescendantsAndSelf(units, p.DesignationUnitId)).ToList();
            if (organizationUnitId.HasValue && !officerBranchIds.Contains(organizationUnitId.Value)) continue;

            officers.Add(new TrackingFieldOfficer(
                p.EmployeeId,
                p.Code,
                p.FullName,
                p.PostName,
                p.DesignationName,
                p.OrganizationUnitId,
                p.OrganizationUnitName,
                p.DesignationUnitName,
                officerBranchIds,
                p.MobileNo,
                GeoCoordinates.ToGeoPoint(p.PresentLocation),
                p.ReportingOfficerName,
                isBelow));
        }
        return officers.OrderBy(o => o.Name).ToList();
    }

    // An active employee's latest open designation and the branches at or below its unit
    public async Task<TrackingOfficer?> GetOfficerAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var position = await context.TblEmployeeDesignations
            .Where(ed => ed.EmployeeId == employeeId && ed.EffectiveTo == null && ed.Employee.IsActive)
            .OrderByDescending(ed => ed.EffectiveFrom)
            .Select(ed => new { ed.EmployeeId, ed.Employee.FullName, ed.DesignationId, PostName = ed.Designation.Post.Name, ed.Designation.OrganizationUnitId })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (position is null) return null;

        var units = await context.TblOrganizationUnits
            .Select(o => new UnitNode(o.Id, o.ParentId, o.Name, o.OrganizationUnitType.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new TrackingOfficer(
            position.EmployeeId,
            position.FullName,
            position.DesignationId,
            position.PostName,
            position.OrganizationUnitId,
            BranchIdsOf(DescendantsAndSelf(units, position.OrganizationUnitId)).ToList());
    }

    public async Task<Dictionary<Guid, BranchCode>> GetBranchCodesAsync(IEnumerable<Guid> branchIds, CancellationToken cancellationToken = default)
    {
        var ids = branchIds.Distinct().ToList();
        return await context.TblOrganizationUnits
            .Where(o => ids.Contains(o.Id))
            .Select(o => new BranchCode(o.Id, o.Name, o.CodeVal))
            .AsNoTracking()
            .ToDictionaryAsync(b => b.Id, cancellationToken);
    }

    public async Task<DateOnly?> GetFinancialYearStartAsync(DateOnly day, CancellationToken cancellationToken = default) =>
        await context.TblFinancialYears
            .Where(f => f.StartDate <= day && f.EndDate >= day)
            .Select(f => (DateOnly?)f.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

    private sealed record UnitNode(Guid Id, Guid? ParentId, string Name, string TypeName);

    private static UnitNode? AncestorOfType(Dictionary<Guid, UnitNode> units, Guid? startId, string typeName)
    {
        var visited = new HashSet<Guid>();
        while (startId.HasValue && visited.Add(startId.Value) && units.TryGetValue(startId.Value, out var unit))
        {
            if (unit.TypeName == typeName) return unit;
            startId = unit.ParentId;
        }
        return null;
    }

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
