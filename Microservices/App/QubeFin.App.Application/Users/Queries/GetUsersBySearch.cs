using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUsersBySearchQuery(string? SearchText, string? SortOn, string? SortDirection, int PageIndex, int PageSize) : IRequest<Result<GetUsersBySearchResponse>>;
#endregion

#region --- RESPONSE ---
public record UsersBySearchResult(Guid Id, string UserName, string Employee, string MfaSecret, bool HasMfaEnabled, bool IsActive);
public record GetUsersBySearchResponse(IReadOnlyList<UsersBySearchResult> Users, int TotalCount);
#endregion

#region --- HANDLER ---
internal sealed class GetUsersBySearchQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUsersBySearchQuery, Result<GetUsersBySearchResponse>>
{
    public async Task<Result<GetUsersBySearchResponse>> Handle(GetUsersBySearchQuery request, CancellationToken cancellationToken)
    {
        var skipRecordCount = request.PageIndex * request.PageSize;
        var filterEntitiesQuery = context.TblUsers.Include(m => m.Employee).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.UserName.Contains(request.SearchText.Trim()) || m.Employee.FullName.Contains(request.SearchText.Trim()));
        }

        if (request.SortOn is not null && request.SortDirection is not null)
        {
            filterEntitiesQuery = request.SortOn switch
            {
                "username" => request.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.UserName) : filterEntitiesQuery.OrderBy(m => m.UserName),
                _ => request.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.UserName) : filterEntitiesQuery.OrderBy(m => m.UserName),
            };
        }

        var totalCount = await filterEntitiesQuery.CountAsync(cancellationToken);
        var Users = await filterEntitiesQuery.Skip(skipRecordCount).Take(request.PageSize)
            .Select(m => new UsersBySearchResult(m.Id, m.UserName, m.Employee.FullName, m.MfaSecret, m.HasMfaEnabled, m.IsActive))
            .ToListAsync(cancellationToken: cancellationToken);

        return Result.Ok(new GetUsersBySearchResponse(Users, totalCount));
    }
}
#endregion