using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceHistoryByQuery(AttendanceSearchRequest searchParam, Guid employeeId) : IRequest<GetAllAttendanceHistoryResponse>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceHistoryByQueryValidator : AbstractValidator<GetAttendanceHistoryByQuery>
{
    public GetAttendanceHistoryByQueryValidator()
    {
        RuleFor(v => v.searchParam).NotNull().WithMessage("Search parameters are required.");
        RuleFor(v => v.searchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("PageIndex must be greater than or equal to 0.");
        RuleFor(v => v.searchParam.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetAllAttendanceHistoryResponse(IReadOnlyList<AttendanceSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceHistoryByQueryHandler(QubeFinDataContext context, IMemoryCache cache) : IRequestHandler<GetAttendanceHistoryByQuery, GetAllAttendanceHistoryResponse>
{
    private const string OrgUnitCacheKey = "org-units-flat";
    private static readonly TimeSpan OrgUnitCacheTtl = TimeSpan.FromMinutes(10);
    public async Task<GetAllAttendanceHistoryResponse> Handle(GetAttendanceHistoryByQuery request, CancellationToken cancellationToken)
    {
        var organizationUnitIds = await ResolveOrganizationUnitIdsAsync(request.employeeId, cancellationToken);

        var query = BuildAttendanceQuery(request.searchParam, organizationUnitIds);
        query = ApplySearch(query, request.searchParam.SearchText);
        query = ApplyStatusFilter(query, request.searchParam.Status);
        query = ApplySort(query, request.searchParam.SortOn, request.searchParam.SortDirection);

        var total = await query.CountAsync(cancellationToken);

        var skip = request.searchParam.PageIndex * request.searchParam.PageSize;
        var data = await query
            .Skip(skip)
            .Take(request.searchParam.PageSize)
            .ToListAsync(cancellationToken);

        var attendances = data.Select(MapToResult).ToList();

        return new GetAllAttendanceHistoryResponse(attendances, total);
    }

    // ---- Query building -------------------------------------------------

    private IQueryable<TblAttendance> BuildAttendanceQuery(
        AttendanceSearchRequest searchParam,
        List<Guid> organizationUnitIds)
    {
        var baseQuery = context.TblAttendances
            .Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit)
            .Where(a => a.Employee.OrganizationUnitId != null
                        && organizationUnitIds.Contains(a.Employee.OrganizationUnitId.Value))
            .AsNoTracking();

        var hasFrom = searchParam.FromDate.HasValue;
        var hasTo = searchParam.ToDate.HasValue;

        if (!hasFrom && !hasTo)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return baseQuery.Where(a => a.AttendanceDate == today);
        }

        if (hasFrom)
            baseQuery = baseQuery.Where(a => a.AttendanceDate >= searchParam.FromDate!.Value);

        if (hasTo)
            baseQuery = baseQuery.Where(a => a.AttendanceDate <= searchParam.ToDate!.Value);

        return baseQuery;
    }

    private static IQueryable<TblAttendance> ApplySearch(IQueryable<TblAttendance> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return query;

        var text = searchText.Trim();
        return query.Where(m =>
            m.Employee.FullName.Contains(text) ||
            m.Employee.Code.Contains(text) ||
            (m.Employee.OrganizationUnit != null && m.Employee.OrganizationUnit.Name.Contains(text)));
    }

    private static IQueryable<TblAttendance> ApplyStatusFilter(IQueryable<TblAttendance> query, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return query;

        return status.Trim().ToLowerInvariant() switch
        {
            "on time" => query.Where(m => !m.IsLateEntry && !m.IsEarlyLeave),
            "late entry" => query.Where(m => m.IsLateEntry && !m.IsEarlyLeave),
            "early exit" => query.Where(m => !m.IsLateEntry && m.IsEarlyLeave),
            "late entry & early exit" => query.Where(m => m.IsLateEntry && m.IsEarlyLeave),
            _ => query
        };
    }

    private static IQueryable<TblAttendance> ApplySort(
        IQueryable<TblAttendance> query,
        string? sortOn,
        string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortOn) || string.IsNullOrWhiteSpace(sortDirection))
            return query.OrderByDescending(m => m.AttendanceDate); // sensible default

        var ascending = sortDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase);

        return sortOn.Trim().ToLowerInvariant() switch
        {
            "employeename" => ascending
                ? query.OrderBy(m => m.Employee.FullName)
                : query.OrderByDescending(m => m.Employee.FullName),
            "employeecode" => ascending
                ? query.OrderBy(m => m.Employee.Code)
                : query.OrderByDescending(m => m.Employee.Code),
            "attendancedate" or _ => ascending
                ? query.OrderBy(m => m.AttendanceDate)
                : query.OrderByDescending(m => m.AttendanceDate)
        };
    }

    // ---- Mapping ----------------------------------------------------------

    private static AttendanceSearchResult MapToResult(TblAttendance m) => new()
    {
        Id = m.Id,
        OrganizationUnit = m.Employee.OrganizationUnit?.Name,
        EmployeeName = m.Employee.FullName,
        EmployeeCode = m.Employee.Code,
        AttendanceDate = m.AttendanceDate,
        ActualInTime = m.ActualInTime.ToString("h:mm tt"),
        ActualOutTime = m.ActualOutTime?.ToString("h:mm tt"),
        WorkingHours = GetWorkingHours(m.ActualInTime, m.ActualOutTime),
        Status = GetAttendanceStatus(m.AttendanceDate, m.ActualInTime, m.ActualOutTime, m.IsLateEntry, m.IsEarlyLeave),
        IsRegularized = m.IsRegularization ? "Yes" : "-"
    };

    private static string? GetWorkingHours(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue || !outTime.HasValue)
            return null;

        var duration = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();

        if (duration < TimeSpan.Zero)
            duration += TimeSpan.FromDays(1); // night shift support

        return $"{duration.Hours} h {duration.Minutes} m";
    }

    private static string GetAttendanceStatus(
        DateOnly attendanceDate,
        TimeOnly? inTime,
        TimeOnly? outTime,
        bool isLateEntry,
        bool isEarlyLeave)
    {
        var isPastDay = attendanceDate < DateOnly.FromDateTime(DateTime.Now.Date);
        if (isPastDay && (inTime == null || outTime == null))
            return "MSP";

        return (isLateEntry, isEarlyLeave) switch
        {
            (false, false) => "On Time",
            (true, false) => "Late Entry",
            (false, true) => "Early Exit",
            (true, true) => "Late Entry & Early Exit"
        };
    }

    // ---- Organization unit resolution (cached) -----------------------------

    private async Task<List<Guid>> ResolveOrganizationUnitIdsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee?.OrganizationUnitId is not { } rootUnitId)
            return [];

        var branchIds = await GetBranchIdsUnder(rootUnitId, cancellationToken);
        branchIds.Add(rootUnitId);

        return branchIds.Distinct().ToList();
    }

    private async Task<List<Guid>> GetBranchIdsUnder(Guid orgUnitId, CancellationToken cancellationToken)
    {
        var units = await GetAllOrganizationUnitsCachedAsync(cancellationToken);

        var byParent = units
            .Where(u => u.ParentId.HasValue)
            .ToLookup(u => u.ParentId!.Value);

        var result = new List<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(orgUnitId);

        // Iterative traversal avoids recursion-depth issues on deep org trees.
        var visited = new HashSet<Guid>();
        while (stack.Count > 0)
        {
            var currentId = stack.Pop();
            if (!visited.Add(currentId))
                continue;

            var current = units.FirstOrDefault(u => u.Id == currentId);
            if (current == null)
                continue;

            if (current.OrganizationUnitType.Name == "Branch")
                result.Add(current.Id);

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
