using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Collection.Persistence.Repositories;
using QubeFin.Persistence.Models.Collection;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
// Single loan of one BC group of a credit officer (designation) for one collection date.
public record GetGroupWiseLoanDetailQuery(
    Guid EmployeeId,
    Guid DesignationId,
    Guid BcGroupId,
    Guid LoanId) : IRequest<Result<GroupWiseLoanResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupWiseLoanDetailQueryValidator : AbstractValidator<GetGroupWiseLoanDetailQuery>
{
    public GetGroupWiseLoanDetailQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
        RuleFor(v => v.BcGroupId).NotEmpty().WithMessage("BC Group Id is required.");
        RuleFor(v => v.LoanId).NotEmpty().WithMessage("Loan Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetGroupWiseLoanDetailQueryHandler(ICollectionRepository collectionRepository)
    : IRequestHandler<GetGroupWiseLoanDetailQuery, Result<GroupWiseLoanResponse>>
{
    public async Task<Result<GroupWiseLoanResponse>> Handle(GetGroupWiseLoanDetailQuery request, CancellationToken cancellationToken)
    {
        var m = await collectionRepository.GetGroupLoanDetailByLoanIdAsync(
            request.EmployeeId,
            request.DesignationId,
            request.BcGroupId,
            request.LoanId,
            cancellationToken);

        if (m is null)
        {
            return Result.Fail<GroupWiseLoanResponse>("Loan not found for the given credit officer, group and organization unit.");
        }

        return Result.Ok(new GroupWiseLoanResponse(
            m.Id, m.LoanNo, m.BcGroupId, m.GroupCode, m.GroupName,
            m.BcMemberId, m.MemberCode, m.MemberName, m.MemberMobileNo,
            m.LoanProductName, m.LoanPurpose, m.LoanAmount, m.RateOfInterest, m.LoanTenure,
            m.EmiAmount, m.DisburseDate, m.DisburseAmount, m.TotalPaidAmount, m.TotalPaidInstallment,
            m.CurrentOutstandingAmount, m.OverdueAmount, m.DpdDays, m.IsClosed, m.ClosedDate,
            m.CollectionId, m.CollectionEmiAmount, m.CollectedAmount,
            m.IsDemandDate, m.IsCollected, m.CollectionRemarks));
    }
}
#endregion