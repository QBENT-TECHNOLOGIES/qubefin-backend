using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetMenuByIdQuery(Guid Id) : IRequest<Result<GetMenuByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetMenuByIdResponse(Guid Id, string Name, string Icon, string? Target, Guid? ParentId, int DisplayPosition, bool IsActive, IReadOnlyList<MenuHierarchyItem> Hierarchy);
#endregion

#region --- HANDLER ---
internal sealed class GetMenuByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMenuByIdQuery, Result<GetMenuByIdResponse>>
{
    public async Task<Result<GetMenuByIdResponse>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var hierarchy = await context.Set<MenuHierarchyItem>()
            .FromSqlInterpolated($@"
                ;WITH Hierarchy AS
                (
                    SELECT
                        m.Id,
                        m.Name,
                        m.Icon,
                        m.Target,
                        m.ParentId,
                        0 AS Level
                    FROM [Auth].[Tbl_Menu] m
                    WHERE M.Id = {request.Id}
                    UNION ALL

                    SELECT
                        p.Id,
                        p.Name,
                        p.Icon,
                        p.Target,
                        p.ParentId,
                        h.Level + 1
                    FROM [Auth].[Tbl_Menu] p
                    INNER JOIN Hierarchy h
                        ON h.ParentId = p.Id
                )
                SELECT Id, Name, Icon, Target, Level
                FROM Hierarchy
                ORDER BY Level DESC
                ")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var menu = await context
            .TblMenus
            .AsNoTracking()
            .Where(m => m.Id == request.Id)
            .Select(m => new GetMenuByIdResponse(m.Id, m.Name, m.Icon, m.Target, m.ParentId, m.DisplayPosition, m.IsActive, hierarchy))
            .FirstOrDefaultAsync(cancellationToken);

        if (menu is null)
        {
            return new RecordNotFoundError($"Menu not found for the given Id");
        }
        return Result.Ok(menu);
    }
}
#endregion