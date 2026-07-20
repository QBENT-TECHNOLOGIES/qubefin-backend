using FluentValidation;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.SurveyUnit.Queries;

#region --- QUERY ---
public record GetSurveyBySearchQuery(SurveySearchParam searchParam) : IRequest<GetSurveyBySearchResponse>;
#endregion

#region --- RESPONSE ---
public record GetSurveyBySearchResponse(IReadOnlyList<SurveyResults> results, int TotalRecords);
#endregion

#region --- VALIDATOR ---
public class SurveyQueryValidator : AbstractValidator<GetSurveyBySearchQuery>
{
    public SurveyQueryValidator()
    {
        RuleFor(x => x.searchParam.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index should be >= 0.");
        RuleFor(x => x.searchParam.PageSize).GreaterThanOrEqualTo(0).WithMessage("Page size should be >= 0.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyBySearchQueryHandler(QubeFinDataContext context) : IRequestHandler<GetSurveyBySearchQuery, GetSurveyBySearchResponse>
{
    public async Task<GetSurveyBySearchResponse> Handle(GetSurveyBySearchQuery request, CancellationToken cancellationToken)
    {
        var surveySearchResults = await context.Set<SurveyResults>()
          .FromSqlRaw("EXEC [Global].[USP_FilteredSurvey] @SearchText, @SortOn, @SortDirection, @PageIndex, @PageSize",
            new SqlParameter("@SearchText", request.searchParam.SearchText ?? ""),
            new SqlParameter("@SortOn", request.searchParam.SortOn ?? ""),
            new SqlParameter("@SortDirection", request.searchParam.SortDirection ?? ""),
            new SqlParameter("@PageIndex", request.searchParam.PageIndex),
            new SqlParameter("@PageSize", request.searchParam.PageSize)
            )
          .AsNoTracking()
          .ToListAsync(cancellationToken);
        int totalRecords = surveySearchResults.Count > 0 ? surveySearchResults[0].TotalCount : 0;
        return new GetSurveyBySearchResponse(surveySearchResults, totalRecords);
    }
}
#endregion
