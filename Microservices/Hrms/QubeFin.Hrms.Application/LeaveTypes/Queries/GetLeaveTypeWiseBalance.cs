using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;
public record GetLeaveTypeWiseBalanceQuery(Guid employeeId) : IRequest<Result<List<GetLeaveTypeWiseBalanceResponse>>>;

public record GetLeaveTypeWiseBalanceResponse(Guid? Id, string? Name, string? Alias, decimal OpenningBalance, decimal? LeaveCredit, decimal? LeaveDebit,decimal? CurrentBalance);
internal sealed class GetLeaveTypeWiseBalanceQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLeaveTypeWiseBalanceQuery, Result<List<GetLeaveTypeWiseBalanceResponse>>>
{
    public async Task<Result<List<GetLeaveTypeWiseBalanceResponse>>> Handle(GetLeaveTypeWiseBalanceQuery request, CancellationToken cancellationToken)
    {
        var leaveTypeWiseBalanceResponse = await context.Set<LeaveTypeWiseBalanceResponse>().FromSqlRaw("EXEC [Hrms].[USP_LeaveTypeWiseBalance] @EmployeeId",
         new SqlParameter("@EmployeeId", request.employeeId)
        )
       .AsNoTracking()
       .ToListAsync(cancellationToken);
        return Result.Ok(leaveTypeWiseBalanceResponse.Select(m => new GetLeaveTypeWiseBalanceResponse(m.Id, m.Title, m.Alias, 0, m.LeaveCredit, m.LeaveDebit, m.CurrentBalance)).OrderBy(m => m.Name).ToList());
    }
}
