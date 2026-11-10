using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
public record GetBcLoanBySearchTextQuery(string SearchText) : IRequest<Result<List<GetBcLoanBySearchTextResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetBcLoanBySearchTextResponse(Guid Id, string LoanNo);
#endregion

#region --- HANDLER ---
internal sealed class GetBcLoanBySearchTextQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetBcLoanBySearchTextQuery, Result<List<GetBcLoanBySearchTextResponse>>>
{
    public async Task<Result<List<GetBcLoanBySearchTextResponse>>> Handle(GetBcLoanBySearchTextQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchText))
        {
            return Result.Ok(new List<GetBcLoanBySearchTextResponse>());
        }

        var searchResult = await context.TblBcloans
            .AsNoTracking()
            .Where(m =>!m.IsClosed && m.LoanNo.Contains(request.SearchText))
            .OrderBy(m => m.LoanNo)
            .Select(m => new GetBcLoanBySearchTextResponse(
                m.Id,
                m.LoanNo
            ))
            .ToListAsync(cancellationToken);

        return Result.Ok(searchResult);
    }
}
#endregion