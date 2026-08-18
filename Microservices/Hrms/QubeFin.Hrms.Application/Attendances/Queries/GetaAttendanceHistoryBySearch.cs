using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceHistoryByQuery(AttendanceSearchRequest searchParam) : IRequest<GetAllAttendanceHistoryResponse>;
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
internal sealed class GetAttendanceHistoryByQueryHandler(QubeFinDataContext context) : IRequestHandler<GetAttendanceHistoryByQuery, GetAllAttendanceHistoryResponse>
{
    public async Task<GetAllAttendanceHistoryResponse> Handle(GetAttendanceHistoryByQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var query = request.searchParam.FromDate == null && request.searchParam.ToDate == null ?
            context.TblAttendances.Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit).AsNoTracking().AsQueryable() :
            request.searchParam.FromDate.HasValue && request.searchParam.ToDate.HasValue ?
            context.TblAttendances.Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit).Where(x => x.AttendanceDate >= request.searchParam.FromDate.Value && x.AttendanceDate <= request.searchParam.ToDate.Value).AsNoTracking().AsQueryable() :
            request.searchParam.FromDate.HasValue ?
            context.TblAttendances.Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit).Where(x => x.AttendanceDate >= request.searchParam.FromDate.Value).AsNoTracking().AsQueryable() :
            request.searchParam.ToDate.HasValue ?
            context.TblAttendances.Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit).Where(x => x.AttendanceDate <= request.searchParam.ToDate.Value).AsNoTracking().AsQueryable() :
            context.TblAttendances.Include(a => a.Employee).ThenInclude(e => e.OrganizationUnit).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.searchParam.SearchText))
        {
            query = query.Where(m => m.Employee.FullName.Contains(request.searchParam.SearchText) || m.Employee.Code.Contains(request.searchParam.SearchText) || m.Employee.OrganizationUnit.Name.Contains(request.searchParam.SearchText));
        }

        if (!string.IsNullOrWhiteSpace(request.searchParam.Status))
        {
            var status = request.searchParam.Status.Trim().ToLowerInvariant();
            query = status switch
            {
                "on time" => query.Where(m => !m.IsLateEntry && !m.IsEarlyLeave),
                "late entry" => query.Where(m => m.IsLateEntry && !m.IsEarlyLeave),
                "early exit" => query.Where(m => !m.IsLateEntry && m.IsEarlyLeave),
                "late entry & early exit" => query.Where(m => m.IsLateEntry && m.IsEarlyLeave),
                _ => query
            };
        }

        if (request.searchParam.SortOn is not null && request.searchParam.SortDirection is not null)
        {
            query = request.searchParam.SortOn switch
            {
                _ => request.searchParam.SortDirection == "ASC" ? query.OrderBy(m => m.AttendanceDate) : query.OrderByDescending(m => m.AttendanceDate),
            };
        }

        var total = await query.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var skip = request.searchParam.PageIndex * request.searchParam.PageSize;

        var data = await query
            .Skip(skip)
            .Take(request.searchParam.PageSize)
            .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var attendances = data.Select(m => new AttendanceSearchResult
        {
            Id = m.Id,
            OrganizationUnit = m.Employee.OrganizationUnit?.Name,
            EmployeeName = m.Employee.FullName,
            EmployeeCode = m.Employee.Code,
            AttendanceDate = m.AttendanceDate,
            ActualInTime = m.ActualInTime.ToString("h:mm tt"),
            ActualOutTime = m.ActualOutTime?.ToString("h:mm tt"),
            WorkingHours = GetWorkingHours(m.ActualInTime, m.ActualOutTime),
            Status = GetAttendanceStatus(m.IsLateEntry, m.IsEarlyLeave),
            IsRegulerized = m.IsRegularization ? "Yes" : "-"
        }).ToList();

        return new GetAllAttendanceHistoryResponse(attendances, total);
    }

    private static string? GetWorkingHours(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue || !outTime.HasValue)
            return null;

        var duration = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();

        if (duration < TimeSpan.Zero)
            duration += TimeSpan.FromDays(1); // Night shift support

        return $"{duration.Hours} hours {duration.Minutes} minutes";
    }
    private static string GetAttendanceStatus(bool IsLateEntry, bool IsEarlyLeave)
    {
        return (IsLateEntry, IsEarlyLeave) switch
        {
            (false, false) => "On Time",
            (true, false) => "Late Entry",
            (false, true) => "Early Exit",
            (true, true) => "Late Entry & Early Exit"
        };
    }
}
#endregion
