using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

public record GetLeaveWiseTransactionQuery(Guid employeeId, Guid LeaveTypeId) : IRequest<Result<List<GetLeaveWiseTransactionResponse>>>;

public record GetLeaveWiseTransactionResponse(Guid? TransactionId, DateOnly? TransactionDate, decimal? LeaveCredit , decimal? LeaveDebit, string? Remarks);
internal sealed class GetLeaveWiseTransactionQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLeaveWiseTransactionQuery, Result<List<GetLeaveWiseTransactionResponse>>>
{
    public async Task<Result<List<GetLeaveWiseTransactionResponse>>> Handle(GetLeaveWiseTransactionQuery request, CancellationToken cancellationToken)
    {
        var leaveTypeWiseBalanceResponse = await context.TblLeaveTransactions.Where(m => m.LeaveTypeId == request.LeaveTypeId && m.EmployeeId == request.employeeId && m.LeaveYear == DateTime.Now.Year)
       .AsNoTracking().ToListAsync(cancellationToken);

        return Result.Ok(leaveTypeWiseBalanceResponse.Select(m => new GetLeaveWiseTransactionResponse(m.Id, m.TransactionDate, m.LeaveCredit, m.LeaveDebit, m.Remarks)).OrderBy(m => m.TransactionDate).ToList());
    }
}
