using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.LeaveTypes.Queries;

public record GetLeaveWiseTransactionQuery(Guid employeeId, Guid LeaveTypeId) : IRequest<Result<List<GetLeaveWiseTransactionResponse>>>;

public record GetLeaveWiseTransactionResponse(Guid? TransactionId, DateOnly? TransactionDate, decimal? LeaveCredit, decimal? LeaveDebit, string? Remarks);
internal sealed class GetLeaveWiseTransactionQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLeaveWiseTransactionQuery, Result<List<GetLeaveWiseTransactionResponse>>>
{
    public async Task<Result<List<GetLeaveWiseTransactionResponse>>> Handle(GetLeaveWiseTransactionQuery request, CancellationToken cancellationToken)
    {
        var currentYear = DateTime.Now.Year;
        var yearStart = new DateTime(currentYear, 1, 1);

        var transactions = await context.TblLeaveTransactions.Where(m => m.LeaveTypeId == request.LeaveTypeId && m.EmployeeId == request.employeeId && m.LeaveYear == currentYear)
            .AsNoTracking().OrderBy(m => m.TransactionDate).ToListAsync(cancellationToken);

        var openingTransaction = transactions.FirstOrDefault(m => m.TransactionDate == DateOnly.FromDateTime(yearStart));

        var response = transactions.Where(m => openingTransaction == null || m.Id != openingTransaction.Id)
            .Select(m => new GetLeaveWiseTransactionResponse(m.Id, m.TransactionDate, m.LeaveCredit, m.LeaveDebit, m.Remarks))
            .OrderBy(m => m.TransactionDate).ToList();


        response.Insert(0, new GetLeaveWiseTransactionResponse(Guid.Empty, DateOnly.FromDateTime(yearStart), openingTransaction == null ? 0 : openingTransaction.LeaveCredit, 0, "Opening Balance"));


        return Result.Ok(response);
    }
}
