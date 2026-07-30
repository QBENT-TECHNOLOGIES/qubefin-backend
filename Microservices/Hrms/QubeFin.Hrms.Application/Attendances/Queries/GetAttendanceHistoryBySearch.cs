using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceHistoryByQuery(Guid EmployeeId, AttendanceSearchRequest searchParam) : IRequest<GetAttendanceHistoryResponse>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceHistoryByQueryValidator : AbstractValidator<GetAttendanceHistoryByQuery>
{
    public GetAttendanceHistoryByQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotNull().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetAttendanceHistoryResponse(IReadOnlyList<AttendanceSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetAttendanceHistoryByEmployeeQueryHandler(QubeFinDataContext context) : IRequestHandler<GetAttendanceHistoryByQuery, GetAttendanceHistoryResponse>
{
    public async Task<GetAttendanceHistoryResponse> Handle(GetAttendanceHistoryByQuery request, CancellationToken cancellationToken)
    {
        var query = context.TblAttendances.AsNoTracking().Where(m => m.EmployeeId == request.EmployeeId).AsQueryable();

        if (request.searchParam.FromDate.HasValue)
        {
            query = query.Where(m => m.AttendanceDate >= request.searchParam.FromDate.Value);
        }
        if (request.searchParam.ToDate.HasValue)
        {
            query = query.Where(m => m.AttendanceDate <= request.searchParam.ToDate.Value);
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
            AttendanceDate = m.AttendanceDate,
            ActualInTime = m.ActualInTime.ToString("h:mm tt"),
            ActualOutTime = m.ActualOutTime?.ToString("h:mm tt"),
            WorkingHours = GetWorkingHours(m.ActualInTime, m.ActualOutTime),
            Status = GetAttendanceStatus(m.IsLateEntry, m.IsEarlyLeave)
        }).ToList();

        return new GetAttendanceHistoryResponse(attendances, total);
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
