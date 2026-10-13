using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Report.Application.Reports.Models;

namespace QubeFin.Report.Application.Reports.Queries;

#region --- QUERY ---
public record GetAttendanceExportQuery(AttendanceExportRequest searchParam, Guid employeeId)
    : IRequest<Result<List<AttendanceSearchResult>>>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceExportQueryValidator : AbstractValidator<GetAttendanceExportQuery>
{
    public GetAttendanceExportQueryValidator()
    {
        RuleFor(v => v.employeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.searchParam).NotNull().WithMessage("Search parameters are required.");
        RuleFor(v => v.searchParam.CompanyId)
            .NotEmpty()
            .When(v => v.searchParam is not null)
            .WithMessage("Company Id is required.");

      
    }
}
#endregion

#region --- HANDLER ---

internal sealed class GetAttendanceExportQueryHandler(
    QubeFinDataContext context,
    IMemoryCache cache,
    ILogger<GetAttendanceExportQueryHandler> logger)
    : IRequestHandler<GetAttendanceExportQuery, Result<List<AttendanceSearchResult>>>
{
    private const string OrgUnitCacheKey = "org-units-flat";
    private static readonly TimeSpan OrgUnitCacheTtl = TimeSpan.FromMinutes(10);
    private const int MaxAttendanceDays = 31;
    private const string Dash = "-";

   
    private static class RowStatus
    {
        public const string OnTime = "On Time";
        public const string LateEntry = "Late Entry";
        public const string EarlyExit = "Early Exit";
        public const string LateEntryAndEarlyExit = "Late Entry & Early Exit";
        public const string Missed = "MSP";
        public const string OnLeave = "On Leave";
        public const string Absent = "Absent";
        public const string NotPunched = "Not Punched";

        private static readonly HashSet<string> Known = new(StringComparer.OrdinalIgnoreCase)
        {
            OnTime, LateEntry, EarlyExit, LateEntryAndEarlyExit, Missed, OnLeave, Absent, NotPunched
        };

        public static bool IsKnown(string? status) => status is not null && Known.Contains(status.Trim());

        public static bool IsPending(string status) =>
            status is Missed or Absent or NotPunched;
    }

    private const string ApprovedLeaveStatus = "Approved";

    private sealed record AttendanceEmployee(
        Guid Id, string FullName, string Code, string? OrganizationUnit, DateOnly? JoiningDate, DateOnly? SeparationDate)
    {
        public bool IsEmployedOn(DateOnly day) =>
            (JoiningDate is null || JoiningDate <= day) && (SeparationDate is null || SeparationDate >= day);
    }

    private sealed record AttendanceEntry(
        Guid EmployeeId, DateOnly AttendanceDate, TimeOnly? ActualInTime, TimeOnly? ActualOutTime,
        bool IsLateEntry, bool IsEarlyLeave, bool IsRegularization);

    private sealed record AttendanceLeave(Guid EmployeeId, DateOnly FromDate, DateOnly ToDate);

    private sealed record AttendanceRow(AttendanceEmployee Employee, DateOnly Date, AttendanceEntry? Entry, string Status);

    public async Task<Result<List<AttendanceSearchResult>>> Handle(
        GetAttendanceExportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var param = request.searchParam;

            var scope = await ResolveOrganizationUnitIdsAsync(request.employeeId, cancellationToken);
            if (scope.Count == 0)
                return Result.Ok(new List<AttendanceSearchResult>());

            var (from, to) = ResolveWindow(param);
            var employeeQuery = BuildEmployeeQuery(param, scope, from, to);

            var employees = await employeeQuery
                .Select(m => new AttendanceEmployee(
                    m.Id, m.FullName, m.Code,
                    m.OrganizationUnit != null ? m.OrganizationUnit.Name : null,
                    m.JoiningDate, m.SeparationDate))
                .ToListAsync(cancellationToken);

            if (employees.Count == 0)
                return Result.Ok(new List<AttendanceSearchResult>());

            var entries = await context.TblAttendances
                .AsNoTracking()
                .Where(a => a.AttendanceDate >= from && a.AttendanceDate <= to
                            && employeeQuery.Select(m => m.Id).Contains(a.EmployeeId))
                .Select(a => new AttendanceEntry(
                    a.EmployeeId, a.AttendanceDate, a.ActualInTime, a.ActualOutTime,
                    a.IsLateEntry, a.IsEarlyLeave, a.IsRegularization))
                .ToListAsync(cancellationToken);

            var entryLookup = entries
                .GroupBy(a => (a.EmployeeId, a.AttendanceDate))
                .ToDictionary(g => g.Key, g => g.First());

            var leaves = await context.TblLeaveRequests
                .AsNoTracking()
                .Where(l => l.CurrentStatus == ApprovedLeaveStatus
                            && l.FromDate <= to && l.ToDate >= from
                            && employeeQuery.Select(m => m.Id).Contains(l.EmployeeId))
                .Select(l => new AttendanceLeave(l.EmployeeId, l.FromDate, l.ToDate))
                .ToListAsync(cancellationToken);

            var leaveLookup = leaves.ToLookup(l => l.EmployeeId);

            var days = Enumerable.Range(0, to.DayNumber - from.DayNumber + 1)
                .Select(from.AddDays)
                .ToList();

            var rows = employees
                .SelectMany(e => days
                    .Where(e.IsEmployedOn)
                    .Select(d =>
                    {
                        entryLookup.TryGetValue((e.Id, d), out var entry);
                        var onLeave = leaveLookup[e.Id].Any(l => l.FromDate <= d && l.ToDate >= d);
                        return new AttendanceRow(e, d, entry, DeriveRowStatus(d, entry, onLeave));
                    }))
                .ToList();

            rows = ApplyStatusFilter(rows, param.Status);

            var result = ApplySort(rows, param.SortOn, param.SortDirection)
                .Select(MapToResult)
                .ToList();

            return Result.Ok(result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to build attendance history for employee {EmployeeId}", request.employeeId);
            return Result.Fail<List<AttendanceSearchResult>>("Something went wrong. Please try again later");
        }
    }

    private static (DateOnly From, DateOnly To) ResolveWindow(AttendanceExportRequest param)
    {
        var today = DateOnly.FromDateTime(DateTime.Now.Date);
        var from = param.FromDate ?? param.ToDate ?? today;
        var to = param.ToDate ?? param.FromDate ?? today;

        if (to < from) (from, to) = (to, from);
        if (to.DayNumber - from.DayNumber + 1 > MaxAttendanceDays)
            from = to.AddDays(-(MaxAttendanceDays - 1));

        return (from, to);
    }

    private IQueryable<TblEmployee> BuildEmployeeQuery(
        AttendanceExportRequest param, List<Guid> scope, DateOnly from, DateOnly to)
    {
        var query = context.TblEmployees
            .AsNoTracking()
            .Where(m => m.IsActive
                        && m.CompanyId == param.CompanyId
                        && m.OrganizationUnitId != null
                        && scope.Contains(m.OrganizationUnitId.Value)
                        && (m.JoiningDate == null || m.JoiningDate <= to)
                        && (m.SeparationDate == null || m.SeparationDate >= from));

        if (param.SearchEmployeeId.HasValue)
            query = query.Where(m => m.Id == param.SearchEmployeeId.Value);

        if (!string.IsNullOrWhiteSpace(param.SearchText))
        {
            var text = param.SearchText.Trim();
            query = query.Where(m => m.FullName.Contains(text)
                                     || m.Code.Contains(text)
                                     || (m.OrganizationUnit != null && m.OrganizationUnit.Name.Contains(text)));
        }

        return query;
    }


    private static string DeriveRowStatus(DateOnly date, AttendanceEntry? entry, bool onLeave)
    {
        var hasPunch = entry is { ActualInTime: not null } or { ActualOutTime: not null };
        if (onLeave && !hasPunch)
            return RowStatus.OnLeave;

        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        if (entry is not null)
        {
            var isPastDay = entry.AttendanceDate < today;
            if (isPastDay && (entry.ActualInTime is null || entry.ActualOutTime is null))
                return RowStatus.Missed;

            return (entry.IsLateEntry, entry.IsEarlyLeave) switch
            {
                (false, false) => RowStatus.OnTime,
                (true, false) => RowStatus.LateEntry,
                (false, true) => RowStatus.EarlyExit,
                (true, true) => RowStatus.LateEntryAndEarlyExit
            };
        }

        return date < today ? RowStatus.Absent : RowStatus.NotPunched;
    }

    private static List<AttendanceRow> ApplyStatusFilter(List<AttendanceRow> rows, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return rows;

        var wanted = status.Trim();

        if (wanted.Equals("all", StringComparison.OrdinalIgnoreCase))
            return rows;

        if (wanted.Equals("pending", StringComparison.OrdinalIgnoreCase))
            return rows.Where(r => RowStatus.IsPending(r.Status)).ToList();

        if (RowStatus.IsKnown(wanted))
            return rows.Where(r => r.Status.Equals(wanted, StringComparison.OrdinalIgnoreCase)).ToList();

        return rows;
    }


    private static IOrderedEnumerable<AttendanceRow> ApplySort(
        IEnumerable<AttendanceRow> rows, string? sortOn, string? sortDirection)
    {
        return sortOn?.Trim().ToLowerInvariant() switch
        {
            "employeename" => OrderRows(rows, r => r.Employee.FullName, sortDirection).ThenBy(r => r.Date),
            "organizationunit" => OrderRows(rows, r => r.Employee.OrganizationUnit ?? string.Empty, sortDirection).ThenBy(r => r.Employee.FullName),
            "employeecode" => OrderRows(rows, r => r.Employee.Code, sortDirection).ThenBy(r => r.Date),
            "status" => OrderRows(rows, r => r.Status, sortDirection).ThenBy(r => r.Employee.FullName),
            _ => OrderRows(rows, r => r.Date, sortDirection).ThenBy(r => r.Employee.FullName)
        };
    }

    private static IOrderedEnumerable<T> OrderRows<T, TKey>(
        IEnumerable<T> rows, Func<T, TKey> keySelector, string? sortDirection)
    {
        var ascending = !string.IsNullOrWhiteSpace(sortDirection)
                        && sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return ascending ? rows.OrderBy(keySelector) : rows.OrderByDescending(keySelector);
    }


    private static AttendanceSearchResult MapToResult(AttendanceRow r) => new()
    {
        OrganizationUnit = r.Employee.OrganizationUnit,
        EmployeeName = r.Employee.FullName,
        EmployeeCode = r.Employee.Code,
        AttendanceDate = r.Date,
        ActualInTime = r.Entry?.ActualInTime?.ToString("h:mm tt") ?? Dash,
        ActualOutTime = r.Entry?.ActualOutTime?.ToString("h:mm tt") ?? Dash,
        WorkingHours = r.Entry is null
            ? Dash
            : GetWorkingHours(r.Entry.ActualInTime, r.Entry.ActualOutTime) ?? Dash,
        Status = r.Status,
        IsRegularized = r.Entry is { IsRegularization: true } ? "Yes" : Dash
    };

    private static string? GetWorkingHours(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue || !outTime.HasValue)
            return null;

        var duration = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();

        if (duration < TimeSpan.Zero)
            duration += TimeSpan.FromDays(1); 

        return $"{duration.Hours} h {duration.Minutes} m";
    }


    private async Task<List<Guid>> ResolveOrganizationUnitIdsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee?.OrganizationUnitId is not { } rootUnitId)
            return [];

        var units = await GetAllOrganizationUnitsCachedAsync(cancellationToken);
        var byParent = units.Where(u => u.ParentId.HasValue).ToLookup(u => u.ParentId!.Value);

        var result = new List<Guid>();
        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(rootUnitId);

        while (stack.Count > 0)
        {
            var currentId = stack.Pop();
            if (!visited.Add(currentId)) continue;

            result.Add(currentId);

            foreach (var child in byParent[currentId])
                stack.Push(child.Id);
        }

        return result;
    }

    private async Task<List<TblOrganizationUnit>> GetAllOrganizationUnitsCachedAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(OrgUnitCacheKey, out List<TblOrganizationUnit>? cached) && cached is not null)
            return cached;

        var units = await context.TblOrganizationUnits
            .Include(u => u.OrganizationUnitType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        cache.Set(OrgUnitCacheKey, units, OrgUnitCacheTtl);
        return units;
    }
}
#endregion