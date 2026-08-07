using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.LeaveApproval.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.LeaveApproval.Queries;

#region --- QUERY ---
public record GetLeaveApprovalsByEmployeeIdQuery(Guid EmployeeId, LeaveApprovalSearchRequest searchParam) : IRequest<GetLeaveApprovalsByEmployeeIdResponse>;
#endregion

#region --- VALIDATOR ---
public class GetLeaveApprovalsByEmployeeIdQueryValidator : AbstractValidator<GetLeaveApprovalsByEmployeeIdQuery>
{
    public GetLeaveApprovalsByEmployeeIdQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotNull().WithMessage("Employee Id is required.");
        RuleFor(v => v.searchParam.PageSize).NotNull().WithMessage("Page Size is required.");
        RuleFor(v => v.searchParam.PageIndex).NotNull().WithMessage("Page Index is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetLeaveApprovalsByEmployeeIdResponse(IReadOnlyList<LeaveApprovalSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetLeaveApprovalsByEmployeeIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetLeaveApprovalsByEmployeeIdQuery, GetLeaveApprovalsByEmployeeIdResponse>
{
    public async Task<GetLeaveApprovalsByEmployeeIdResponse> Handle(GetLeaveApprovalsByEmployeeIdQuery request, CancellationToken cancellationToken)
    {

        var searchResults = await context.Set<LeaveApprovalSearchResult>()
        .FromSqlRaw("EXEC [Hrms].[USP_FilteredLeaveApprovals] @FromDate, @ToDate, @SortOn, @SortDirection, @PageIndex, @PageSize, @EmployeeId, @SearchEmployeeId",
          new SqlParameter("@FromDate", (object?)request.searchParam.FromDate ?? DBNull.Value),
          new SqlParameter("@ToDate", (object?)request.searchParam.ToDate ?? DBNull.Value),
          new SqlParameter("@SortOn", request.searchParam.SortOn ?? ""),
          new SqlParameter("@SortDirection", request.searchParam.SortDirection ?? ""),
          new SqlParameter("@PageIndex", request.searchParam.PageIndex),
          new SqlParameter("@PageSize", request.searchParam.PageSize),
          new SqlParameter("@EmployeeId", request.EmployeeId),
          new SqlParameter("@SearchEmployeeId", (object?)request.searchParam.SearchEmployeeId ?? DBNull.Value)
        )
        .AsNoTracking()
        .ToListAsync(cancellationToken);
        int? totalRecords = searchResults.Count > 0 ? searchResults[0].TotalCount : 0;

        return new GetLeaveApprovalsByEmployeeIdResponse(searchResults, totalRecords > 0 ? totalRecords.Value : 0);
    }
}
#endregion
