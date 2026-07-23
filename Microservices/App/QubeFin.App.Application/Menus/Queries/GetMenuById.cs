using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetMenuByIdQuery(Guid Id) : IRequest<Result<GetMenuByIdResponse>>;
#endregion

#region --- RESPONSE ---
//public sealed record GetMenuByIdResponse(Guid Id, string Name, string Icon, string? Target, Guid? ParentId, int DisplayPosition, bool IsActive,
//    string CreatedBy, DateTime CreatedOn, string? LastModifiedBy, DateTime? LastModifiedOn, IReadOnlyList<MenuHierarchyItem> Hierarchy, IReadOnlyList<Permission> Permissions);
public sealed record GetMenuByIdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Target { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public int DisplayPosition { get; init; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
    public string? LastModifiedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public IReadOnlyList<MenuHierarchyItem> Hierarchy { get; init; } = [];
    public IReadOnlyList<PermissionResponse> Permissions { get; init; } = [];
}
public sealed record PermissionResponse
{
    public string PermissionToken { get; init; } = string.Empty;
    public int DisplayPosition { get; init; }
    public bool Checked { get; init; }
}
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
            .Select(m => new GetMenuByIdResponse
            {
                Id = m.Id,
                Name = m.Name,
                Icon = m.Icon,
                Target = m.Target,
                ParentId = m.ParentId,
                DisplayPosition = m.DisplayPosition,
                IsActive = m.IsActive,
                CreatedBy = m.CreatedByNavigation.UserName,
                CreatedOn = m.CreatedOn,
                LastModifiedBy = m.LastModifiedByNavigation != null ? m.LastModifiedByNavigation.UserName : string.Empty,
                LastModifiedOn = m.LastModifiedOn,
                Hierarchy = hierarchy,

                Permissions = m.TblMenuPermissions
                    .OrderBy(x  => x.PermissionTokenNavigation.DisplayPosition)
                    .Select(p => new PermissionResponse
                    {
                        PermissionToken = p.PermissionTokenNavigation.PermissionToken,
                        DisplayPosition = p.PermissionTokenNavigation.DisplayPosition
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (menu is null)
        {
            return new RecordNotFoundError($"Menu not found for the given Id");
        }
        return Result.Ok(menu);
    }
}
#endregion