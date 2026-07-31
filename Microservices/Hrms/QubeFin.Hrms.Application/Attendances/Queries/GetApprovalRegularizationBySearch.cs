using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetApprovalRegularizationBySearch(Guid EmployeeId, AttendanceSearchRequest searchParam) : IRequest<GetApprovalRegularizationBySearchResponse>;
#endregion

#region --- VALIDATOR ---
public class GetApprovalRegularizationBySearchValidator : AbstractValidator<GetApprovalRegularizationBySearch>
{
    public GetApprovalRegularizationBySearchValidator()
    {
        RuleFor(v => v.EmployeeId).NotNull().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetApprovalRegularizationBySearchResponse(IReadOnlyList<RegularizationSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetApprovalRegularizationBySearchHandler(QubeFinDataContext context) : IRequestHandler<GetApprovalRegularizationBySearch, GetApprovalRegularizationBySearchResponse>
{
    public async Task<GetApprovalRegularizationBySearchResponse> Handle(GetApprovalRegularizationBySearch request, CancellationToken cancellationToken)
    {
        var searchResults = await context.Set<RegularizationSearchResult>()
        .FromSqlRaw("EXEC [Hrms].[USP_FilteredRegularizationApprovals] @SearchText, @Status, @FromDate, @ToDate, @SortOn, @SortDirection, @PageIndex, @PageSize, @EmployeeId",
          new SqlParameter("@SearchText", request.searchParam.SearchText ?? ""),
          new SqlParameter("@FromDate", (object?)request.searchParam.FromDate ?? DBNull.Value),
          new SqlParameter("@ToDate", (object?)request.searchParam.ToDate ?? DBNull.Value),
          new SqlParameter("@SortOn", request.searchParam.SortOn ?? ""),
          new SqlParameter("@SortDirection", request.searchParam.SortDirection ?? ""),
          new SqlParameter("@PageIndex", request.searchParam.PageIndex),
          new SqlParameter("@PageSize", request.searchParam.PageSize),
          new SqlParameter("@EmployeeId", request.EmployeeId)
        )
        .AsNoTracking()
        .ToListAsync(cancellationToken);
        int? totalRecords = searchResults.Count > 0 ? searchResults[0].TotalCount : 0;

        return new GetApprovalRegularizationBySearchResponse(searchResults, totalRecords > 0 ? totalRecords.Value : 0);
    }
}
#endregion
