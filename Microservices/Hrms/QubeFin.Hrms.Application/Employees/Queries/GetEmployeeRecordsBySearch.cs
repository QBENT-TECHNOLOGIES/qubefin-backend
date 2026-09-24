using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using System.Text.Json;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeRecordsBySearchQuery(Guid EmployeeId, EmployeeRecordSearchRequest searchParam) : IRequest<GetEmployeeRecordsBySearchResponse>;
#endregion

#region --- VALIDATOR ---
public class GetEmployeeRecordsBySearchQueryValidator : AbstractValidator<GetEmployeeRecordsBySearchQuery>
{
    public GetEmployeeRecordsBySearchQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.searchParam).NotNull().WithMessage("Search parameters are required.");
        RuleFor(v => v.searchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("PageIndex must be greater than or equal to 0.");
        RuleFor(v => v.searchParam.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than 0.");
        RuleFor(v => v.searchParam.RecordType)
            .Must(EmployeeRecordTypes.IsValid)
            .When(v => !string.IsNullOrWhiteSpace(v.searchParam.RecordType))
            .WithMessage($"Record Type must be one of: {string.Join(", ", EmployeeRecordTypes.All)}.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetEmployeeRecordsBySearchResponse(
    IReadOnlyList<EmployeeRecordItem> results,
    int TotalRecords,
    EmployeeRecordCounts Counts,
    EmployeeRecordStatusCounts StatusCounts);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeRecordsBySearchQueryHandler(QubeFinDataContext context, IMemoryCache cache)
    : IRequestHandler<GetEmployeeRecordsBySearchQuery, GetEmployeeRecordsBySearchResponse>
{
    private const string OrgUnitCacheKey = "org-units-flat";
    private static readonly TimeSpan OrgUnitCacheTtl = TimeSpan.FromMinutes(10);
    private const string DateFormat = "dd MMM yyyy";

    public async Task<GetEmployeeRecordsBySearchResponse> Handle(GetEmployeeRecordsBySearchQuery request, CancellationToken cancellationToken)
    {
        var recordType = EmployeeRecordTypes.Normalize(request.searchParam.RecordType);
        var scope = await ResolveOrganizationUnitIdsAsync(request.EmployeeId, cancellationToken);
        var param = request.searchParam;

        var (results, totalRecords, statusCounts) = recordType switch
        {
            EmployeeRecordTypes.Regularization => await LoadRegularizationAsync(param, scope, cancellationToken),
            EmployeeRecordTypes.Prayer => await LoadPrayerAsync(param, scope, cancellationToken),
            EmployeeRecordTypes.Attendance => await LoadAttendanceAsync(param, scope, cancellationToken),
            EmployeeRecordTypes.Fitness => await LoadFitnessAsync(param, scope, cancellationToken),
            _ => await LoadLeaveAsync(param, scope, cancellationToken)
        };

        var counts = param.IncludeCounts
            ? await LoadCountsAsync(param, scope, cancellationToken)
            : new EmployeeRecordCounts();

        return new GetEmployeeRecordsBySearchResponse(results, totalRecords, counts, statusCounts);
    }

    // ---- Leave requests ----------------------------------------------------

    private IQueryable<TblLeaveRequest> LeaveQuery(EmployeeRecordSearchRequest param, List<Guid> scope)
    {
        var query = context.TblLeaveRequests
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.LeaveType)
            .AsNoTracking()
            .Where(m => m.Employee.OrganizationUnitId != null && scope.Contains(m.Employee.OrganizationUnitId.Value));

        if (param.CompanyId.HasValue)
            query = query.Where(m => m.Employee.CompanyId == param.CompanyId.Value);

        if (param.SearchEmployeeId.HasValue)
            query = query.Where(m => m.EmployeeId == param.SearchEmployeeId.Value);

        // A leave overlaps the window when it starts before the end and finishes after the start.
        if (param.FromDate.HasValue)
            query = query.Where(m => m.ToDate >= param.FromDate.Value);

        if (param.ToDate.HasValue)
            query = query.Where(m => m.FromDate <= param.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(param.SearchText))
        {
            var text = param.SearchText.Trim();
            query = query.Where(m => m.Employee.FullName.Contains(text)
                                     || m.Employee.Code.Contains(text)
                                     || (m.Employee.OrganizationUnit != null && m.Employee.OrganizationUnit.Name.Contains(text))
                                     || m.LeaveType.Title.Contains(text));
        }

        return query;
    }

    private async Task<(IReadOnlyList<EmployeeRecordItem>, int, EmployeeRecordStatusCounts)> LoadLeaveAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken)
    {
        var baseQuery = LeaveQuery(param, scope);
        var statusCounts = await CountStatusesAsync(baseQuery.Select(m => m.CurrentStatus), cancellationToken);

        var query = FilterStatus(baseQuery, param.Status);
        var total = await query.CountAsync(cancellationToken);

        var sorted = (param.SortOn?.Trim().ToLowerInvariant()) switch
        {
            "employeename" => Order(query, m => m.Employee.FullName, param.SortDirection),
            "organizationunit" => Order(query, m => m.Employee.OrganizationUnit!.Name, param.SortDirection),
            "status" => Order(query, m => m.CurrentStatus, param.SortDirection),
            "appliedon" => Order(query, m => m.RequestDate, param.SortDirection),
            _ => Order(query, m => m.FromDate, param.SortDirection)
        };

        var rows = await Page(sorted, param).ToListAsync(cancellationToken);

        var items = rows.Select(m => new EmployeeRecordItem
        {
            Id = m.Id,
            RecordType = EmployeeRecordTypes.Leave,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            OrganizationUnit = m.Employee.OrganizationUnit != null ? m.Employee.OrganizationUnit.Name : null,
            Category = m.LeaveType.Title,
            Period = FormatRange(m.FromDate, m.ToDate),
            Quantity = (m.TotalDays ?? CountDays(m.FromDate, m.ToDate)).ToString(),
            Status = m.CurrentStatus,
            FromDate = m.FromDate,
            ToDate = m.ToDate,
            AppliedOn = DateOnly.FromDateTime(m.RequestDate),
            Days = m.TotalDays ?? CountDays(m.FromDate, m.ToDate),
            Reason = m.Reason
        }).ToList();

        return (items, total, statusCounts);
    }

    // ---- Attendance regularization ------------------------------------------

    private IQueryable<TblAttendanceRegularization> RegularizationQuery(EmployeeRecordSearchRequest param, List<Guid> scope)
    {
        var query = context.TblAttendanceRegularizations
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .AsNoTracking()
            .Where(m => m.Employee.OrganizationUnitId != null && scope.Contains(m.Employee.OrganizationUnitId.Value));

        if (param.CompanyId.HasValue)
            query = query.Where(m => m.Employee.CompanyId == param.CompanyId.Value);

        if (param.SearchEmployeeId.HasValue)
            query = query.Where(m => m.EmployeeId == param.SearchEmployeeId.Value);

        // RegularizationDates is a serialized list, so the window applies to the applied-on date.
        if (param.FromDate.HasValue)
        {
            var from = param.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(m => m.CreatedOn >= from);
        }

        if (param.ToDate.HasValue)
        {
            var to = param.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(m => m.CreatedOn <= to);
        }

        if (!string.IsNullOrWhiteSpace(param.SearchText))
        {
            var text = param.SearchText.Trim();
            query = query.Where(m => m.Employee.FullName.Contains(text)
                                     || m.Employee.Code.Contains(text)
                                     || (m.Employee.OrganizationUnit != null && m.Employee.OrganizationUnit.Name.Contains(text))
                                     || m.RegularizationType.Contains(text));
        }

        return query;
    }

    private async Task<(IReadOnlyList<EmployeeRecordItem>, int, EmployeeRecordStatusCounts)> LoadRegularizationAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken)
    {
        var baseQuery = RegularizationQuery(param, scope);
        var statusCounts = await CountStatusesAsync(baseQuery.Select(m => m.CurrentStatus), cancellationToken);

        var query = FilterStatus(baseQuery, param.Status);
        var total = await query.CountAsync(cancellationToken);

        var sorted = (param.SortOn?.Trim().ToLowerInvariant()) switch
        {
            "employeename" => Order(query, m => m.Employee.FullName, param.SortDirection),
            "organizationunit" => Order(query, m => m.Employee.OrganizationUnit!.Name, param.SortDirection),
            "status" => Order(query, m => m.CurrentStatus, param.SortDirection),
            _ => Order(query, m => m.CreatedOn, param.SortDirection)
        };

        var rows = await Page(sorted, param).ToListAsync(cancellationToken);

        var items = rows.Select(m => new EmployeeRecordItem
        {
            Id = m.Id,
            RecordType = EmployeeRecordTypes.Regularization,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            OrganizationUnit = m.Employee.OrganizationUnit != null ? m.Employee.OrganizationUnit.Name : null,
            Category = m.RegularizationType,
            Period = NormalizeRegularizationDates(m.RegularizationDates),
            Quantity = FormatTimeRange(m.ActualInTime, m.ActualOutTime),
            Status = m.CurrentStatus,
            AppliedOn = DateOnly.FromDateTime(m.CreatedOn),
            Reason = m.Reason
        }).ToList();

        return (items, total, statusCounts);
    }

    // ---- Leave prayer --------------------------------------------------------

    private IQueryable<TblLeavePrayer> PrayerQuery(EmployeeRecordSearchRequest param, List<Guid> scope)
    {
        var query = context.TblLeavePrayers
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.LeaveType)
            .AsNoTracking()
            .Where(m => m.Employee.OrganizationUnitId != null && scope.Contains(m.Employee.OrganizationUnitId.Value));

        if (param.CompanyId.HasValue)
            query = query.Where(m => m.Employee.CompanyId == param.CompanyId.Value);

        if (param.SearchEmployeeId.HasValue)
            query = query.Where(m => m.EmployeeId == param.SearchEmployeeId.Value);

        if (param.FromDate.HasValue)
        {
            var from = param.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(m => m.CreatedOn >= from);
        }

        if (param.ToDate.HasValue)
        {
            var to = param.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(m => m.CreatedOn <= to);
        }

        if (!string.IsNullOrWhiteSpace(param.SearchText))
        {
            var text = param.SearchText.Trim();
            query = query.Where(m => m.Employee.FullName.Contains(text)
                                     || m.Employee.Code.Contains(text)
                                     || (m.Employee.OrganizationUnit != null && m.Employee.OrganizationUnit.Name.Contains(text))
                                     || m.LeaveType.Title.Contains(text));
        }

        return query;
    }

    private async Task<(IReadOnlyList<EmployeeRecordItem>, int, EmployeeRecordStatusCounts)> LoadPrayerAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken)
    {
        var baseQuery = PrayerQuery(param, scope);
        var statusCounts = await CountStatusesAsync(baseQuery.Select(m => m.CurrentStatus), cancellationToken);

        var query = FilterStatus(baseQuery, param.Status);
        var total = await query.CountAsync(cancellationToken);

        var sorted = (param.SortOn?.Trim().ToLowerInvariant()) switch
        {
            "employeename" => Order(query, m => m.Employee.FullName, param.SortDirection),
            "organizationunit" => Order(query, m => m.Employee.OrganizationUnit!.Name, param.SortDirection),
            "status" => Order(query, m => m.CurrentStatus, param.SortDirection),
            _ => Order(query, m => m.CreatedOn, param.SortDirection)
        };

        var rows = await Page(sorted, param).ToListAsync(cancellationToken);

        var items = rows.Select(m => new EmployeeRecordItem
        {
            Id = m.Id,
            RecordType = EmployeeRecordTypes.Prayer,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            OrganizationUnit = m.Employee.OrganizationUnit != null ? m.Employee.OrganizationUnit.Name : null,
            Category = m.LeaveType.Title,
            Period = m.Remarks,
            Quantity = m.PrayerDays.ToString(),
            Status = m.CurrentStatus,
            AppliedOn = DateOnly.FromDateTime(m.CreatedOn),
            Days = m.PrayerDays,
            Reason = m.Remarks
        }).ToList();

        return (items, total, statusCounts);
    }

    // ---- Attendance history ---------------------------------------------------

    private IQueryable<TblAttendance> AttendanceQuery(EmployeeRecordSearchRequest param, List<Guid> scope)
    {
        var query = context.TblAttendances
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .AsNoTracking()
            .Where(m => m.Employee.OrganizationUnitId != null && scope.Contains(m.Employee.OrganizationUnitId.Value));

        if (param.CompanyId.HasValue)
            query = query.Where(m => m.Employee.CompanyId == param.CompanyId.Value);

        if (param.SearchEmployeeId.HasValue)
            query = query.Where(m => m.EmployeeId == param.SearchEmployeeId.Value);

        if (param.FromDate.HasValue)
            query = query.Where(m => m.AttendanceDate >= param.FromDate.Value);

        if (param.ToDate.HasValue)
            query = query.Where(m => m.AttendanceDate <= param.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(param.SearchText))
        {
            var text = param.SearchText.Trim();
            query = query.Where(m => m.Employee.FullName.Contains(text)
                                     || m.Employee.Code.Contains(text)
                                     || (m.Employee.OrganizationUnit != null && m.Employee.OrganizationUnit.Name.Contains(text)));
        }

        return param.Status?.Trim().ToLowerInvariant() switch
        {
            "on time" => query.Where(m => !m.IsLateEntry && !m.IsEarlyLeave),
            "late entry" => query.Where(m => m.IsLateEntry && !m.IsEarlyLeave),
            "early exit" => query.Where(m => !m.IsLateEntry && m.IsEarlyLeave),
            "late entry & early exit" => query.Where(m => m.IsLateEntry && m.IsEarlyLeave),
            _ => query
        };
    }

    private async Task<(IReadOnlyList<EmployeeRecordItem>, int, EmployeeRecordStatusCounts)> LoadAttendanceAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken)
    {
        var query = AttendanceQuery(param, scope);
        var total = await query.CountAsync(cancellationToken);

        var sorted = (param.SortOn?.Trim().ToLowerInvariant()) switch
        {
            "employeename" => Order(query, m => m.Employee.FullName, param.SortDirection),
            "organizationunit" => Order(query, m => m.Employee.OrganizationUnit!.Name, param.SortDirection),
            "employeecode" => Order(query, m => m.Employee.Code, param.SortDirection),
            _ => Order(query, m => m.AttendanceDate, param.SortDirection)
        };

        var rows = await Page(sorted, param).ToListAsync(cancellationToken);

        var items = rows.Select(m => new EmployeeRecordItem
        {
            Id = m.Id,
            RecordType = EmployeeRecordTypes.Attendance,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            OrganizationUnit = m.Employee.OrganizationUnit != null ? m.Employee.OrganizationUnit.Name : null,
            Category = null,
            Period = m.AttendanceDate.ToString(DateFormat),
            Quantity = FormatTimeRange(m.ActualInTime, m.ActualOutTime),
            Status = DeriveAttendanceStatus(m.AttendanceDate, m.ActualInTime, m.ActualOutTime, m.IsLateEntry, m.IsEarlyLeave),
            Stage = m.IsRegularization ? "Yes" : "-",
            FromDate = m.AttendanceDate,
            ToDate = m.AttendanceDate,
            WorkingHours = FormatWorkingHours(m.ActualInTime, m.ActualOutTime)
        }).ToList();

        // Attendance status is derived from punch data rather than a workflow column.
        return (items, total, new EmployeeRecordStatusCounts { All = total });
    }

    // ---- Medical fitness -------------------------------------------------------

    private IQueryable<TblLeaveRequest> FitnessQuery(EmployeeRecordSearchRequest param, List<Guid> scope)
    {
        var query = LeaveQuery(param, scope)
            .Where(m => m.FitnessReportAttachment != null && m.FitnessReportUploadOn != null);

        return query;
    }

    private async Task<(IReadOnlyList<EmployeeRecordItem>, int, EmployeeRecordStatusCounts)> LoadFitnessAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken)
    {
        var baseQuery = FitnessQuery(param, scope);

        var approved = await baseQuery.CountAsync(m => m.IsFitnessReportApproved, cancellationToken);
        var all = await baseQuery.CountAsync(cancellationToken);
        var statusCounts = new EmployeeRecordStatusCounts
        {
            All = all,
            Approved = approved,
            Pending = all - approved
        };

        var query = param.Status?.Trim().ToLowerInvariant() switch
        {
            "approved" => baseQuery.Where(m => m.IsFitnessReportApproved),
            "pending" => baseQuery.Where(m => !m.IsFitnessReportApproved),
            _ => baseQuery
        };

        var total = await query.CountAsync(cancellationToken);

        var sorted = (param.SortOn?.Trim().ToLowerInvariant()) switch
        {
            "employeename" => Order(query, m => m.Employee.FullName, param.SortDirection),
            "organizationunit" => Order(query, m => m.Employee.OrganizationUnit!.Name, param.SortDirection),
            _ => Order(query, m => m.FitnessReportUploadOn, param.SortDirection)
        };

        var rows = await Page(sorted, param).ToListAsync(cancellationToken);

        var items = rows.Select(m => new EmployeeRecordItem
        {
            Id = m.Id,
            RecordType = EmployeeRecordTypes.Fitness,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            OrganizationUnit = m.Employee.OrganizationUnit != null ? m.Employee.OrganizationUnit.Name : null,
            Category = m.LeaveType.Title,
            Period = FormatRange(m.FromDate, m.ToDate),
            Quantity = (m.TotalDays ?? CountDays(m.FromDate, m.ToDate)).ToString(),
            Status = m.IsFitnessReportApproved ? EmployeeRecordStatuses.Approved : EmployeeRecordStatuses.Pending,
            FromDate = m.FromDate,
            ToDate = m.ToDate,
            AppliedOn = m.FitnessReportUploadOn.HasValue ? DateOnly.FromDateTime(m.FitnessReportUploadOn.Value) : null,
            Days = m.TotalDays ?? CountDays(m.FromDate, m.ToDate),
            Attachment = m.FitnessReportAttachment
        }).ToList();

        return (items, total, statusCounts);
    }

    // ---- Tab counts --------------------------------------------------------------

    private async Task<EmployeeRecordCounts> LoadCountsAsync(
        EmployeeRecordSearchRequest param, List<Guid> scope, CancellationToken cancellationToken) => new()
        {
            Leave = await FilterStatus(LeaveQuery(param, scope), param.Status).CountAsync(cancellationToken),
            Regularization = await FilterStatus(RegularizationQuery(param, scope), param.Status).CountAsync(cancellationToken),
            Prayer = await FilterStatus(PrayerQuery(param, scope), param.Status).CountAsync(cancellationToken),
            Attendance = await AttendanceQuery(param, scope).CountAsync(cancellationToken),
            Fitness = await FitnessQuery(param, scope).CountAsync(cancellationToken)
        };

    // ---- Shared query helpers -------------------------------------------------------

    private static EmployeeRecordStatusFilter ParseStatus(string? status) =>
        (status?.Trim().ToLowerInvariant()) switch
        {
            "pending" => EmployeeRecordStatusFilter.Pending,
            "approved" => EmployeeRecordStatusFilter.Approved,
            "rejected" => EmployeeRecordStatusFilter.Rejected,
            "cancelled" => EmployeeRecordStatusFilter.Cancelled,
            _ => EmployeeRecordStatusFilter.All
        };

    private static IQueryable<TblLeaveRequest> FilterStatus(IQueryable<TblLeaveRequest> query, string? status) =>
        ParseStatus(status) switch
        {
            EmployeeRecordStatusFilter.Pending => query.Where(m =>
                m.CurrentStatus != EmployeeRecordStatuses.Approved &&
                m.CurrentStatus != EmployeeRecordStatuses.Rejected &&
                m.CurrentStatus != EmployeeRecordStatuses.Cancelled &&
                m.CurrentStatus != EmployeeRecordStatuses.Lapsed),
            EmployeeRecordStatusFilter.Approved => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Approved),
            EmployeeRecordStatusFilter.Rejected => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Rejected),
            EmployeeRecordStatusFilter.Cancelled => query.Where(m =>
                m.CurrentStatus == EmployeeRecordStatuses.Cancelled ||
                m.CurrentStatus == EmployeeRecordStatuses.Lapsed),
            _ => query
        };

    private static IQueryable<TblAttendanceRegularization> FilterStatus(IQueryable<TblAttendanceRegularization> query, string? status) =>
        ParseStatus(status) switch
        {
            EmployeeRecordStatusFilter.Pending => query.Where(m =>
                m.CurrentStatus != EmployeeRecordStatuses.Approved &&
                m.CurrentStatus != EmployeeRecordStatuses.Rejected &&
                m.CurrentStatus != EmployeeRecordStatuses.Cancelled &&
                m.CurrentStatus != EmployeeRecordStatuses.Lapsed),
            EmployeeRecordStatusFilter.Approved => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Approved),
            EmployeeRecordStatusFilter.Rejected => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Rejected),
            EmployeeRecordStatusFilter.Cancelled => query.Where(m =>
                m.CurrentStatus == EmployeeRecordStatuses.Cancelled ||
                m.CurrentStatus == EmployeeRecordStatuses.Lapsed),
            _ => query
        };

    private static IQueryable<TblLeavePrayer> FilterStatus(IQueryable<TblLeavePrayer> query, string? status) =>
        ParseStatus(status) switch
        {
            EmployeeRecordStatusFilter.Pending => query.Where(m =>
                m.CurrentStatus != EmployeeRecordStatuses.Approved &&
                m.CurrentStatus != EmployeeRecordStatuses.Rejected &&
                m.CurrentStatus != EmployeeRecordStatuses.Cancelled &&
                m.CurrentStatus != EmployeeRecordStatuses.Lapsed),
            EmployeeRecordStatusFilter.Approved => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Approved),
            EmployeeRecordStatusFilter.Rejected => query.Where(m => m.CurrentStatus == EmployeeRecordStatuses.Rejected),
            EmployeeRecordStatusFilter.Cancelled => query.Where(m =>
                m.CurrentStatus == EmployeeRecordStatuses.Cancelled ||
                m.CurrentStatus == EmployeeRecordStatuses.Lapsed),
            _ => query
        };

    private static async Task<EmployeeRecordStatusCounts> CountStatusesAsync(IQueryable<string> statuses, CancellationToken cancellationToken)
    {
        var grouped = await statuses
            .GroupBy(s => s)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = new EmployeeRecordStatusCounts { All = grouped.Sum(g => g.Count) };

        foreach (var group in grouped)
        {
            switch (group.Status)
            {
                case EmployeeRecordStatuses.Approved:
                    counts.Approved += group.Count;
                    break;
                case EmployeeRecordStatuses.Rejected:
                    counts.Rejected += group.Count;
                    break;
                case EmployeeRecordStatuses.Cancelled:
                case EmployeeRecordStatuses.Lapsed:
                    counts.Cancelled += group.Count;
                    break;
                default:
                    counts.Pending += group.Count;
                    break;
            }
        }

        return counts;
    }

    private static IOrderedQueryable<T> Order<T, TKey>(
        IQueryable<T> query,
        System.Linq.Expressions.Expression<Func<T, TKey>> keySelector,
        string? sortDirection)
    {
        var ascending = !string.IsNullOrWhiteSpace(sortDirection)
                        && sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return ascending ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
    }

    private static IQueryable<T> Page<T>(IQueryable<T> query, EmployeeRecordSearchRequest param) =>
        query.Skip(param.PageIndex * param.PageSize).Take(param.PageSize);

    // ---- Formatting ------------------------------------------------------------------

    private static string? FormatDate(DateOnly? value) => value?.ToString(DateFormat);

    private static string? FormatRange(DateOnly? from, DateOnly? to)
    {
        var start = FormatDate(from);
        var end = FormatDate(to);

        if (start is null) return end;
        if (end is null || start == end) return start;

        return $"{start} – {end}";
    }

    private static string? FormatTimeRange(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue && !outTime.HasValue) return null;

        var start = inTime.HasValue ? inTime.Value.ToString("h:mm tt") : "—";
        var end = outTime.HasValue ? outTime.Value.ToString("h:mm tt") : "—";

        return $"{start} – {end}";
    }

    private static int CountDays(DateOnly from, DateOnly to)
    {
        var days = to.DayNumber - from.DayNumber + 1;
        return days > 0 ? days : 1;
    }

    private static string? FormatWorkingHours(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue || !outTime.HasValue) return null;

        var duration = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();
        if (duration < TimeSpan.Zero) duration += TimeSpan.FromDays(1);

        return $"{duration.Hours} h {duration.Minutes} m";
    }

    private static string DeriveAttendanceStatus(
        DateOnly attendanceDate, TimeOnly? inTime, TimeOnly? outTime, bool isLateEntry, bool isEarlyLeave)
    {
        var isPastDay = attendanceDate < DateOnly.FromDateTime(DateTime.Now.Date);
        if (isPastDay && (inTime is null || outTime is null)) return "MSP";

        return (isLateEntry, isEarlyLeave) switch
        {
            (false, false) => "On Time",
            (true, false) => "Late Entry",
            (false, true) => "Early Exit",
            (true, true) => "Late Entry & Early Exit"
        };
    }

    private static string? NormalizeRegularizationDates(string? dates)
    {
        if (string.IsNullOrWhiteSpace(dates)) return null;

        try
        {
            var parsed = JsonSerializer.Deserialize<List<string>>(dates);
            return parsed is { Count: > 0 } ? string.Join(", ", parsed) : dates;
        }
        catch (JsonException)
        {
            return dates;
        }
    }

    // ---- Organization unit scope (cached) ------------------------------------------------

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

        // Iterative traversal avoids recursion-depth issues on deep org trees.
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
