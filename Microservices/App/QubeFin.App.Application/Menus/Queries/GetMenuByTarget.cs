using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetMenuByTargetQuery(string TargetPath) : IRequest<Result<GetMenuByTargetResponse>>;
#endregion

#region --- RESPONSE ---
public record GetMenuByTargetResponse(Guid Id, string Name, string Icon, string? Target, IReadOnlyList<MenuHierarchyItem> Hierarchy);
#endregion

#region --- HANDLER ---
internal sealed class GetMenuByTargetQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMenuByTargetQuery, Result<GetMenuByTargetResponse>>
{
    public async Task<Result<GetMenuByTargetResponse>> Handle(GetMenuByTargetQuery request, CancellationToken cancellationToken)
    {
        var menuEntity = await context
            .TblMenus
            .AsNoTracking()
            .Where(m => m.Target == request.TargetPath)
            .FirstOrDefaultAsync(cancellationToken);

        if (menuEntity is null)
        {
            return new RecordNotFoundError($"Menu not found for the given TargetPath");
        }

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
                    WHERE M.Id = {menuEntity.Id}
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

        return Result.Ok(new GetMenuByTargetResponse(menuEntity.Id, menuEntity.Name, menuEntity.Icon, menuEntity.Target, hierarchy));
    }
}
#endregion