using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetLeaveCalendarDaysByEmployeeQuery(Guid employeeId, int year, int month) : IRequest<Result<List<GetLeaveCalendarDaysByEmployeeResponse>>>;

public record GetLeaveCalendarDaysByEmployeeResponse(DateOnly? CalendarDate, string? DayName, string? Status, string? LeaveType);
internal sealed class GetLeaveCalendarDaysByEmployeeQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLeaveCalendarDaysByEmployeeQuery, Result<List<GetLeaveCalendarDaysByEmployeeResponse>>>
{
    public async Task<Result<List<GetLeaveCalendarDaysByEmployeeResponse>>> Handle(GetLeaveCalendarDaysByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employeeMonthlyCalendarResponse = await context.Set<EmployeeLeaveMonthlyCalendarResponse>().FromSqlRaw("EXEC [Hrms].[USP_GetEmployeeLeaveMonthlyCalendar] @EmployeeId, @Year, @Month",
         new SqlParameter("@EmployeeId", request.employeeId),
         new SqlParameter("@Year", request.year),
         new SqlParameter("@Month", request.month)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);
        return Result.Ok(employeeMonthlyCalendarResponse.Select(m => new GetLeaveCalendarDaysByEmployeeResponse(m.CalendarDate, m.DayName, m.Status, m.LeaveType)).OrderBy(m => m.CalendarDate).ToList());
    }
}