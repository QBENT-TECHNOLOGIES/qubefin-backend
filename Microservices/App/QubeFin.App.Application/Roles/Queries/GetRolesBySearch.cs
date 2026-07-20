using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Roles.Queries;

#region --- QUERY ---
public record GetRolesBySearchQuery(string? SearchText, string? SortOn, string? SortDirection, int PageIndex, int PageSize) : IRequest<Result<GetRolesBySearchResponse>>;
#endregion

#region --- RESPONSE ---
public record RolesBySearchResult(Guid Id, string Name, bool IsActive);
public record GetRolesBySearchResponse(IReadOnlyList<RolesBySearchResult> Roles, int TotalRecords);
#endregion

#region --- HANDLER ---
internal sealed class GetRolesBySearchQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetRolesBySearchQuery, Result<GetRolesBySearchResponse>>
{
    public async Task<Result<GetRolesBySearchResponse>> Handle(GetRolesBySearchQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.PageIndex * request.PageSize;
        var filterEntitiesQuery = context.TblRoles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.Name.Contains(request.SearchText.Trim()));
        }

        if (request.SortOn is not null && request.SortDirection is not null)
        {
            filterEntitiesQuery = request.SortOn switch
            {
                "name" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Name) : filterEntitiesQuery.OrderBy(m => m.Name),
                _ => request.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Name) : filterEntitiesQuery.OrderBy(m => m.Name),
            };
        }

        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken);
        var Roles = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.PageSize)
            .Select(m => new RolesBySearchResult(m.Id, m.Name, m.IsActive))
            .ToListAsync(cancellationToken: cancellationToken);

        return Result.Ok( new GetRolesBySearchResponse(Roles, totalCount));
    }
}
#endregion
