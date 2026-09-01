using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---

public record GetMenuByIdQuery(Guid Id)
    : IRequest<Result<GetMenuByIdResponse>>;

#endregion

#region --- RESPONSE ---

public sealed record GetMenuByIdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string? Target { get; init; }
    public string? ParentName { get; init; }
    public Guid? ParentId { get; init; }
    public int DisplayPosition { get; init; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedOn { get; init; }
    public string? LastModifiedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public IReadOnlyList<MenuHierarchyItem> Hierarchy { get; init; } = [];
    public IReadOnlyList<PermissionResponse> Permissions { get; init; } = [];
    public List<RoleMenuAssignmentResponse> Roles { get; init; } = [];
    public List<UserMenuAssignmentResponse> Users { get; init; } = [];
}

public sealed class UserMenuAssignmentResponse
{
    public Guid UserId { get; init; }
    public Guid? EmployeeId { get; init; }
    public string UserName { get; init; } = string.Empty;
}

public sealed class RoleMenuAssignmentResponse
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public bool IsSelected { get; init; }
}

public sealed record PermissionResponse
{
    public Guid Id { get; init; }
    public string PermissionToken { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string BackgroundClass { get; init; } = string.Empty;
    public string IconClass { get; init; } = string.Empty;
    public int DisplayPosition { get; init; }
    public bool Checked { get; init; }
}

#endregion

#region --- HANDLER ---

internal sealed class GetMenuByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetMenuByIdQuery, Result<GetMenuByIdResponse>>
{
    public async Task<Result<GetMenuByIdResponse>> Handle(
        GetMenuByIdQuery request,
        CancellationToken cancellationToken)
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


        // =========================================================
        // MENU DETAIL
        // =========================================================

        var menu = await context
            .TblMenus
            .AsNoTracking()
            .Where(m => m.Id == request.Id)
            .Select(m => new GetMenuByIdResponse
            {
                // -------------------------------------------------
                // MENU INFORMATION
                // -------------------------------------------------

                Id = m.Id,

                Name = m.Name,

                Icon = m.Icon,

                Target = m.Target,

                ParentId = m.ParentId,
                ParentName = m.Parent.Name,

                DisplayPosition = m.DisplayPosition,

                IsActive = m.IsActive,


                // -------------------------------------------------
                // AUDIT INFORMATION
                // -------------------------------------------------

                CreatedBy = m.CreatedByNavigation.UserName,

                CreatedOn = m.CreatedOn,

                LastModifiedBy =
                    m.LastModifiedByNavigation != null
                        ? m.LastModifiedByNavigation.UserName
                        : string.Empty,

                LastModifiedOn = m.LastModifiedOn,


                // -------------------------------------------------
                // HIERARCHY
                // -------------------------------------------------

                Hierarchy = hierarchy,


                // -------------------------------------------------
                // APPLICABLE PERMISSIONS
                // -------------------------------------------------

                Permissions = m.TblMenuPermissions
                    .OrderBy(x => x.Permission.DisplayPosition)
                    .Select(p => new PermissionResponse
                    {
                        Id = p.Permission.Id,

                        PermissionToken =
                            p.Permission.PermissionToken,

                        Description =
                            p.Permission.Description,

                        Icon =
                            p.Permission.Icon,

                        BackgroundClass =
                            p.Permission.BackgroundClass,

                        IconClass =
                            p.Permission.IconClass,

                        DisplayPosition =
                            p.Permission.DisplayPosition
                    })
                    .ToList(),


                // =================================================
                // ROLE PERMISSIONS
                // =================================================

                // Return ALL active roles.
                //
                // IsSelected = true when the role has at least
                // one permission for this menu.
                //
                // MenuPermissionIds contains the permissions
                // assigned to this particular role for this menu.

                Roles = context.TblRoles
                    .Where(r => r.IsActive)
                    .Select(r => new RoleMenuAssignmentResponse
                    {
                        RoleId = r.Id,

                        RoleName = r.Name,

                        IsSelected =
                            r.TblRoleMenuPermissions
                                .Any(rmp =>
                                    m.TblMenuPermissions
                                        .Select(mp => mp.Id)
                                        .Contains(
                                            rmp.MenuPermissionId))
                    })
                    .ToList(),


                // =================================================
                // USER / EMPLOYEE PERMISSIONS
                // =================================================

                // Return only users who currently have at least
                // one permission assigned to this menu.

                Users = context.TblUsers
                    .Where(u =>
                        u.IsActive &&
                        u.TblUserMenuPermissions
                            .Any(ump =>
                                m.TblMenuPermissions
                                    .Select(mp => mp.Id)
                                    .Contains(
                                        ump.MenuPermissionId)))
                    .Select(u => new UserMenuAssignmentResponse
                    {
                        UserId = u.Id,
                        EmployeeId = u.EmployeeId,
                        UserName = u.UserName,
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);


        // =========================================================
        // NOT FOUND
        // =========================================================

        if (menu is null)
        {
            return new RecordNotFoundError("Menu not found for the given Id");
        }


        // =========================================================
        // SUCCESS
        // =========================================================

        return Result.Ok(menu);
    }
}

#endregion