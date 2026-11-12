using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
// One BC loan with its member and group, found by the loan id alone.
public record GetBcLoanDetailQuery(Guid LoanId) : IRequest<Result<BcLoanDetailResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetBcLoanDetailQueryValidator : AbstractValidator<GetBcLoanDetailQuery>
{
    public GetBcLoanDetailQueryValidator()
    {
        RuleFor(v => v.LoanId).NotEmpty().WithMessage("Loan Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record BcLoanDetailResponse(
    Guid Id,
    string LoanNo,
    Guid BcGroupId,
    string GroupCode,
    string GroupName,
    Guid BcMemberId,
    string MemberCode,
    string MemberName,
    string? MemberMobileNo,
    string? LoanProductName,
    string? LoanPurpose,
    decimal? LoanAmount,
    decimal? RateOfInterest,
    int? LoanTenure,
    decimal? EmiAmount,
    DateOnly? DisburseDate,
    decimal? DisburseAmount,
    decimal? TotalPaidAmount,
    int? TotalPaidInstallment,
    decimal? CurrentOutstandingAmount,
    decimal? OverdueAmount,
    int? DpdDays,
    bool IsClosed,
    DateOnly? ClosedDate);
#endregion

#region --- HANDLER ---
internal sealed class GetBcLoanDetailQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetBcLoanDetailQuery, Result<BcLoanDetailResponse>>
{
    public async Task<Result<BcLoanDetailResponse>> Handle(GetBcLoanDetailQuery request, CancellationToken cancellationToken)
    {
        var data = await context.TblBcloans
            .AsNoTracking()
            .Where(l => l.Id == request.LoanId)
            .Join(context.TblBcmembers,
                l => l.BcmemberId,
                m => m.Id,
                (l, m) => new { Loan = l, Member = m })
            .Join(context.TblBcgroups,
                x => x.Member.BcgroupId,
                g => g.Id,
                (x, g) => new { x.Loan, x.Member, Group = g })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
        {
            return Result.Fail<BcLoanDetailResponse>("Loan not found.");
        }

        // Locals are named loan / member / bcGroup so they never clash with the lambda parameters (l, m, g, x) above.
        var loan = data.Loan;
        var member = data.Member;
        var bcGroup = data.Group;

        var memberName = string.Join(' ', new[] { member.FirstName, member.MiddleName, member.LastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim()));

        return Result.Ok(new BcLoanDetailResponse(
            loan.Id,
            loan.LoanNo,
            bcGroup.Id,
            bcGroup.Code,
            bcGroup.Name,
            member.Id,
            member.Code,
            memberName,
            member.MobileNo,
            loan.LoanProductName,
            loan.LoanPurpose,
            loan.LoanAmount,
            loan.RateOfInterest,
            loan.LoanTenure,
            loan.Emiamount,
            loan.DisburseDate,
            loan.DisburseAmount,
            loan.TotalPaidAmount,
            loan.TotalPaidInstallment,
            loan.CurrentOutstadingAmount, // DB column typo
            loan.OverdueAmount,
            loan.Dpddays,
            loan.IsClosed,
            loan.ClosedDate));
    }
}
#endregion