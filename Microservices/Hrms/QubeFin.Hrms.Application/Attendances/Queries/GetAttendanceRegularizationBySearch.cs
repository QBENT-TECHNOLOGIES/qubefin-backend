using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Attendances.Queries;

#region --- QUERY ---
public record GetAttendanceRegularizationBySearch(Guid EmployeeId, AttendanceSearchRequest searchParam) : IRequest<GetAttendanceRegularizationBySearchResponse>;
#endregion

#region --- VALIDATOR ---
public class GetAttendanceRegularizationBySearchValidator : AbstractValidator<GetAttendanceRegularizationBySearch>
{
    public GetAttendanceRegularizationBySearchValidator()
    {
        RuleFor(v => v.EmployeeId).NotNull().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record GetAttendanceRegularizationBySearchResponse(IReadOnlyList<RegularizationSearchResult> results, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetRegularizationHistoryBySearchHandler(QubeFinDataContext context) : IRequestHandler<GetAttendanceRegularizationBySearch, GetAttendanceRegularizationBySearchResponse>
{
    public async Task<GetAttendanceRegularizationBySearchResponse> Handle(GetAttendanceRegularizationBySearch request, CancellationToken cancellationToken)
    {
        var searchResults = await context.Set<RegularizationSearchResult>()
        .FromSqlRaw("EXEC [Hrms].[USP_FilteredAttendanceRegularization] @SearchText, @Status, @FromDate, @ToDate, @SortOn, @SortDirection, @PageIndex, @PageSize, @EmployeeId",
          new SqlParameter("@SearchText", request.searchParam.SearchText ?? ""),
          new SqlParameter("@Status", request.searchParam.Status ?? ""),
          new SqlParameter("@FromDate", request.searchParam.FromDate.ToString() ?? ""),
          new SqlParameter("@ToDate", request.searchParam.ToDate.ToString() ?? ""),
          new SqlParameter("@SortOn", request.searchParam.SortOn ?? ""),
          new SqlParameter("@SortDirection", request.searchParam.SortDirection ?? ""),
          new SqlParameter("@PageIndex", request.searchParam.PageIndex),
          new SqlParameter("@PageSize", request.searchParam.PageSize),
          new SqlParameter("@EmployeeId", request.EmployeeId)
          )
        .AsNoTracking()
        .ToListAsync(cancellationToken);
        int? totalRecords = searchResults.Count > 0 ? searchResults[0].TotalCount : 0;

        return new GetAttendanceRegularizationBySearchResponse(searchResults, totalRecords > 0 ? totalRecords.Value : 0);
    }
}
#endregion
