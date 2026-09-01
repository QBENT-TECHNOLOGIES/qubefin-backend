using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Application.Menus.Models;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---

public record GetMenuByIdQuery(Guid Id) : IRequest<Result<GetMenuResponse>>;

#endregion

#region --- HANDLER ---

internal sealed class GetMenuByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetMenuByIdQuery, Result<GetMenuResponse>>
{
    public async Task<Result<GetMenuResponse>> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        // =========================================================
        // MENU HIERARCHY
        // =========================================================

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
                    WHERE m.Id = {request.Id}

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
                SELECT
                    Id,
                    Name,
                    Icon,
                    Target,
                    Level
                FROM Hierarchy
                ORDER BY Level DESC
            ")
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var menu = await context.TblMenus.AsNoTracking().Where(m => m.Id == request.Id)
            .Select(m => new GetMenuResponse
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

                Permissions = m.TblMenuPermissions.OrderBy(x => x.Permission.DisplayPosition).Select(p => new PermissionResponse
                {
                    Id = p.Id,
                    PermissionToken = p.Permission.PermissionToken,
                    Description = p.Permission.Description,
                    Icon = p.Permission.Icon,
                    BackgroundClass = p.Permission.BackgroundClass,
                    IconClass = p.Permission.IconClass,
                    DisplayPosition = p.Permission.DisplayPosition
                })
                .ToList(),

                Roles = context.TblRoles.Where(r => r.IsActive)
                    .Select(r => new RoleMenuAssignmentResponse
                    {
                        RoleId = r.Id,
                        RoleName = r.Name,
                        MenuPermissionIds = r.TblRoleMenuPermissions.Where(rmp => m.TblMenuPermissions.Select(mp => mp.Id).Contains(rmp.MenuPermissionId)).Select(rmp => rmp.MenuPermissionId).ToList(),
                        IsSelected = r.TblRoleMenuPermissions.Any(rmp => m.TblMenuPermissions.Select(mp => mp.Id).Contains(rmp.MenuPermissionId))
                    }).ToList(),

                Users = context.TblUsers.Where(u => u.IsActive && u.TblUserMenuPermissions.Any(ump => m.TblMenuPermissions.Select(mp => mp.Id).Contains(ump.MenuPermissionId)))
                    .Select(u => new UserMenuAssignmentResponse
                    {
                        UserId = u.Id,
                        EmployeeId = u.EmployeeId,
                        UserName = u.UserName,
                        MenuPermissionIds = u.TblUserMenuPermissions.Where(ump => m.TblMenuPermissions.Select(mp => mp.Id).Contains(ump.MenuPermissionId)).Select(ump => ump.MenuPermissionId).ToList()
                    })
                    .ToList()
            }).FirstOrDefaultAsync(cancellationToken);


        if (menu is null)
        {
            return new RecordNotFoundError("Menu not found for the given Id");
        }

        return Result.Ok(menu);
    }
}

#endregion