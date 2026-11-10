using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Collection.Persistence.Repositories;
using QubeFin.Persistence.Models.Collection;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
// EmployeeId is the logged-in user. DesignationId is the credit officer's designation from the credit officer list -
// BC groups are tagged to the designation, not to the employee. BcGroupId narrows the search to one of that officer's
// groups and is optional; null searches every group of the officer. CollectionDate is the sheet being worked on.
public record GetGroupWiseLoansQuery(
    Guid EmployeeId,
    Guid DesignationId,
    Guid? BcGroupId,
    DateOnly CollectionDate,
    BcGroupLoanSearchParam SearchParam) : IRequest<Result<GetGroupWiseLoansResponse>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupWiseLoansQueryValidator : AbstractValidator<GetGroupWiseLoansQuery>
{
    public GetGroupWiseLoansQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
        RuleFor(v => v.BcGroupId).NotEmpty().When(v => v.BcGroupId.HasValue).WithMessage("BC Group Id is required.");
        RuleFor(v => v.CollectionDate).NotEqual(default(DateOnly)).WithMessage("Collection date is required.");
        RuleFor(v => v.SearchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index cannot be negative.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetGroupWiseLoansResponse(IReadOnlyList<GroupWiseLoanResponse> Loans, int TotalRecords);

public record GroupWiseLoanResponse(
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
    DateOnly? ClosedDate,
    Guid? CollectionId,
    decimal? CollectionEmiAmount,
    decimal? CollectedAmount,
    bool IsDemandDate,
    bool IsCollected,
    string? CollectionRemarks);
#endregion

#region --- HANDLER ---
internal sealed class GetGroupWiseLoansQueryHandler(ICollectionRepository collectionRepository)
    : IRequestHandler<GetGroupWiseLoansQuery, Result<GetGroupWiseLoansResponse>>
{
    public async Task<Result<GetGroupWiseLoansResponse>> Handle(GetGroupWiseLoansQuery request, CancellationToken cancellationToken)
    {
        var rows = await collectionRepository.GetGroupLoansAsync(
            request.EmployeeId,
            request.DesignationId,
            request.BcGroupId,
            request.CollectionDate,
            request.SearchParam,
            cancellationToken);

        // Every row of a page carries the same pre-paging total; no rows means nothing matched.
        var totalRecords = rows.Count > 0 ? rows[0].TotalRecords : 0;

        var loans = rows.Select(m => new GroupWiseLoanResponse(
            m.Id,
            m.LoanNo,
            m.BcGroupId,
            m.GroupCode,
            m.GroupName,
            m.BcMemberId,
            m.MemberCode,
            m.MemberName,
            m.MemberMobileNo,
            m.LoanProductName,
            m.LoanPurpose,
            m.LoanAmount,
            m.RateOfInterest,
            m.LoanTenure,
            m.EmiAmount,
            m.DisburseDate,
            m.DisburseAmount,
            m.TotalPaidAmount,
            m.TotalPaidInstallment,
            m.CurrentOutstandingAmount,
            m.OverdueAmount,
            m.DpdDays,
            m.IsClosed,
            m.ClosedDate,
            m.CollectionId,
            m.CollectionEmiAmount,
            m.CollectedAmount,
            m.IsDemandDate,
            m.IsCollected,
            m.CollectionRemarks)).ToList();

        return Result.Ok(new GetGroupWiseLoansResponse(loans, totalRecords));
    }
}
#endregion
