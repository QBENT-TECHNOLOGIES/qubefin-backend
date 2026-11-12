using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Tracking;
using System.Text.Json;

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
    private const string AllBranches = "All branches";

    // Tracking.USP_GetTrackingViewer: the user, their post and unit, and the units and branches in their scope
    public async Task<TrackingViewer?> GetViewerAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = (await context.Set<TrackingViewerResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetTrackingViewer] @UserId = {userId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .FirstOrDefault();
        if (user is null) return null;

        if (user.IsSuperAdmin)
        {
            return new TrackingViewer(
                user.UserId,
                user.EmployeeId,
                user.EmployeeName ?? user.UserName,
                TrackingRole.Admin,
                null,
                AllBranches,
                IdsOf(user.UnitIdsCsv),
                IdsOf(user.BranchIdsCsv));
        }

        if (user.EmployeeId is null) return null;
        if (user.PostName is null || user.UnitId is null || !TrackingRole.IsTrackingPost(user.PostName)) return null;

        return new TrackingViewer(
            user.UserId,
            user.EmployeeId,
            user.EmployeeName ?? user.UserName,
            user.PostName,
            user.UnitId,
            user.UnitName ?? string.Empty,
            IdsOf(user.UnitIdsCsv),
            IdsOf(user.BranchIdsCsv));
    }

    // Tracking.USP_GetTrackingBranches: the branches with code, nearest Area and Division and location, sorted by name
    public async Task<List<TrackingBranch>> GetBranchesAsync(IReadOnlyList<Guid> branchIds, CancellationToken cancellationToken = default)
    {
        var branches = await context.Set<TrackingBranchResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetTrackingBranches] @BranchIds = {CsvOf(branchIds)}")
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var setting = await GetTrackingSettingAsync(cancellationToken);

        return branches
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

    // Tracking.USP_GetTrackingSetting: the active Tbl_TrackingSetting row (latest if more than one is active)
    public async Task<TblTrackingSetting?> GetTrackingSettingAsync(CancellationToken cancellationToken = default) =>
        (await context.TblTrackingSettings
            .FromSqlRaw("EXEC [Tracking].[USP_GetTrackingSetting]")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .FirstOrDefault();

    // Global.USP_GetFieldOfficers: everyone in a lower role inside the user's scope (plus a Branch Manager's own Assistant
    // Branch Manager), with the branches each person covers
    public async Task<List<TrackingFieldOfficer>> GetFieldOfficersAsync(TrackingViewer viewer, Guid? organizationUnitId, CancellationToken cancellationToken = default)
    {
        var rows = await context.Set<FieldOfficerResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetFieldOfficers] @UserId = {viewer.UserId}, @OrganisationUnitId = {organizationUnitId}")
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
            string.IsNullOrEmpty(r.OrganisationUnitsJson)
                ? []
                : JsonSerializer.Deserialize<List<OrganizationUnits>>(r.OrganisationUnitsJson, JsonOptions) ?? [],
            r.Phone,
            r.PresentLatitude.HasValue && r.PresentLongitude.HasValue
                ? GeoCoordinates.ToGeoPoint((decimal)r.PresentLatitude.Value, (decimal)r.PresentLongitude.Value)
                : null,
            r.ReportingOfficerName,
            r.CanAssign)).ToList();
    }

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    // Tracking.USP_GetTrackingOfficer: an active employee's latest open designation and the branches at or below its unit
    public async Task<TrackingOfficer?> GetOfficerAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var officer = (await context.Set<TrackingOfficerResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetTrackingOfficer] @EmployeeId = {employeeId}")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .FirstOrDefault();
        if (officer is null) return null;

        return new TrackingOfficer(
            officer.EmployeeId,
            officer.FullName,
            officer.DesignationId,
            officer.PostName,
            officer.UnitId,
            IdsOf(officer.BranchIdsCsv));
    }

    // Tracking.USP_GetBranchCodes
    public async Task<Dictionary<Guid, BranchCode>> GetBranchCodesAsync(IEnumerable<Guid> branchIds, CancellationToken cancellationToken = default)
    {
        var ids = branchIds.Distinct().ToList();
        return (await context.Set<BranchCodeResult>()
            .FromSqlInterpolated($"EXEC [Tracking].[USP_GetBranchCodes] @BranchIds = {CsvOf(ids)}")
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .ToDictionary(b => b.Id, b => new BranchCode(b.Id, b.Name, b.CodeVal));
    }

    // Tracking.USP_GetFinancialYearStart
    public async Task<DateOnly?> GetFinancialYearStartAsync(DateOnly day, CancellationToken cancellationToken = default)
    {
        var starts = await context.Database
            .SqlQuery<DateOnly>($"EXEC [Tracking].[USP_GetFinancialYearStart] @Day = {day}")
            .ToListAsync(cancellationToken);
        return starts.Count == 0 ? null : starts[0];
    }

    private static string CsvOf(IEnumerable<Guid> ids) => string.Join(',', ids);

    private static List<Guid> IdsOf(string? csv) =>
        string.IsNullOrEmpty(csv)
            ? []
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Guid.Parse).ToList();
}
