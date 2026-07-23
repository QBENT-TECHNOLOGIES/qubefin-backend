using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetParentMenusQuery : IRequest<Result<List<GetParentMenusResponse>>>;
#endregion

#region ---RESPONSE ---
public record GetParentMenusResponse(Guid Id, string Name, string Icon);
#endregion

#region --- HANDLER ---
internal sealed class GetParentMenusQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetParentMenusQuery, Result<List<GetParentMenusResponse>>>
{
    public async Task<Result<List<GetParentMenusResponse>>> Handle(GetParentMenusQuery request, CancellationToken cancellationToken)
    {
        var menus = await context.TblMenus
            .AsNoTracking()
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.DisplayPosition)
            .Select(m => new GetParentMenusResponse(m.Id, m.Name, m.Icon))
            .ToListAsync(cancellationToken);

        return Result.Ok(menus);
    }
}
#endregion