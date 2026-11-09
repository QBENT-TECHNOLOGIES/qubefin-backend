using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;

namespace QubeFin.Tracking.Persistence.Repositories;

public interface ITrackingScopeRepository
{
    Task<TrackingViewer?> GetViewerAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<TrackingBranch>> GetUserWiseBranchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TblTrackingSetting?> GetTrackingSettingAsync(CancellationToken cancellationToken = default);
    Task<List<TrackingFieldOfficer>> GetFieldOfficersAsync(Guid userId, Guid? organizationUnitId, CancellationToken cancellationToken = default);
    Task<TrackingOfficer?> GetOfficerAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, BranchCode>> GetBranchCodesAsync(IEnumerable<Guid> branchIds, CancellationToken cancellationToken = default);
    Task<DateOnly?> GetFinancialYearStartAsync(DateOnly day, CancellationToken cancellationToken = default);
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

    // The user's branches with their details, from Global.Fn_GetUserWiseBranches; the operating radius comes from Tbl_TrackingSetting
    public async Task<List<TrackingBranch>> GetUserWiseBranchesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var branches = await context.Set<UserWiseBranchResult>()
            .FromSqlInterpolated($"SELECT * FROM [Global].[Fn_GetUserWiseBranches]({userId})")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var setting = await GetTrackingSettingAsync(cancellationToken);

        return branches
            .DistinctBy(b => b.Id)
            .OrderBy(b => b.Name)
            .Select(b => new TrackingBranch(
                b.Id,
                b.Name,
                b.Code,
                b.AreaName,
                b.DivisionName,
                GeoCoordinates.ToGeoPoint(b.Latitude, b.Longitude),
                setting?.BranchOperatingRadiusInKm))
            .ToList();
    }

    // The active Tbl_TrackingSetting row (latest if more than one is active)
    public async Task<TblTrackingSetting?> GetTrackingSettingAsync(CancellationToken cancellationToken = default) =>
        await context.TblTrackingSettings
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.ModifiedOn ?? s.CreatedOn)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    // Global.USP_GetFieldOfficers: everyone in a lower role whose branches are all inside the user's branches
    // (Global.Fn_GetUserWiseBranches), plus a Branch Manager's own Assistant Branch Manager
    public async Task<List<TrackingFieldOfficer>> GetFieldOfficersAsync(Guid userId, Guid? organizationUnitId, CancellationToken cancellationToken = default)
    {
        var rows = await context.Set<FieldOfficerResult>()
            .FromSqlInterpolated($"EXEC [Global].[USP_GetFieldOfficers] @UserId = {userId}, @OrganisationUnitId = {organizationUnitId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rows.Select(r => new TrackingFieldOfficer(
            r.Id,
            r.EmployeeCode,
            r.Name,
            r.Role,
            r.Designation,
            r.OrganisationUnitId,
            r.OrganisationUnitName,
            r.UnitName,
            (r.OrganisationUnitIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse).ToList(),
            r.Phone,
            r.PresentLatitude.HasValue && r.PresentLongitude.HasValue
                ? GeoCoordinates.ToGeoPoint((decimal)r.PresentLatitude.Value, (decimal)r.PresentLongitude.Value)
                : null,
            r.ReportingOfficerName,
            r.CanAssign)).ToList();
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
