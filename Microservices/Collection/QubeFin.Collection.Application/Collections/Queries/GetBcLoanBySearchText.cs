using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
public record GetBcLoanBySearchTextQuery(Guid DesignationId, Guid BcGroupId, string? SearchText) : IRequest<Result<List<GetBcLoanBySearchTextResponse>>>;
#endregion
#region --- VALIDATOR ---
public class GetBcLoanBySearchTextQueryValidator : AbstractValidator<GetBcLoanBySearchTextQuery>
{
    public GetBcLoanBySearchTextQueryValidator()
    {
        RuleFor(v => v.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
        RuleFor(v => v.BcGroupId).NotEmpty().WithMessage("BC Group Id is required.");
    }
}
#endregion
#region --- RESPONSE ---
public record GetBcLoanBySearchTextResponse(Guid Id, string LoanNo, string MemberName);
#endregion

#region --- HANDLER ---
internal sealed class GetBcLoanBySearchTextQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetBcLoanBySearchTextQuery, Result<List<GetBcLoanBySearchTextResponse>>>
{
    private const int MaxResults = 20;

    public async Task<Result<List<GetBcLoanBySearchTextResponse>>> Handle(
        GetBcLoanBySearchTextQuery request,
        CancellationToken cancellationToken)
    {
        var searchText = request.SearchText?.Trim();

        var query = context.TblBcloans
            .AsNoTracking()
            .Where(loan => !loan.IsClosed
                        && loan.Bcmember.BcgroupId == request.BcGroupId
                        && loan.Bcmember.Bcgroup.DesignationId == request.DesignationId);

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(loan => loan.LoanNo.Contains(searchText));
        }

        var rows = await query
            .OrderBy(loan => loan.LoanNo)
            .Take(MaxResults)
            .Select(loan => new
            {
                loan.Id,
                loan.LoanNo,
                loan.Bcmember.FirstName,
                loan.Bcmember.MiddleName,
                loan.Bcmember.LastName
            })
            .ToListAsync(cancellationToken);

        var loans = rows
            .Select(row => new GetBcLoanBySearchTextResponse(
                row.Id,
                row.LoanNo,
                BuildFullName(row.FirstName, row.MiddleName, row.LastName)))
            .ToList();

        return Result.Ok(loans);
    }

    private static string BuildFullName(params string?[] parts) =>
        string.Join(' ', parts
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim()));
}
#endregion