using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Queries;

public record GetCalendarDaysByEmployeeQuery(Guid employeeId, int year, int month) : IRequest<Result<List<GetCalendarDaysByEmployeeResponse>>>;

public record GetCalendarDaysByEmployeeResponse(DateOnly? CalendarDate, string? DayName, string? Status);
internal sealed class GetCalendarDaysByEmployeeQueryHandler(QubeFinDataContext context) : IRequestHandler<GetCalendarDaysByEmployeeQuery, Result<List<GetCalendarDaysByEmployeeResponse>>>
{
    public async Task<Result<List<GetCalendarDaysByEmployeeResponse>>> Handle(GetCalendarDaysByEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employeeMonthlyCalendarResponse = await context.Set<EmployeeMonthlyCalendarResponse>().FromSqlRaw("EXEC [Hrms].[USP_GetEmployeeMonthlyCalendar] @EmployeeId, @Year, @Month",
         new SqlParameter("@EmployeeId", request.employeeId),
         new SqlParameter("@Year", request.year),
         new SqlParameter("@Month", request.month)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);
        return Result.Ok(employeeMonthlyCalendarResponse.Select(m => new GetCalendarDaysByEmployeeResponse(m.CalendarDate, m.DayName, m.Status)).OrderBy(m => m.CalendarDate).ToList());
    }
}