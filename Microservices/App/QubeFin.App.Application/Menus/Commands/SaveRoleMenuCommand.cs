using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Application.Menus.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.App.Application.Menus.Commands;

#region --- COMMAND ---

public sealed record SaveRoleMenuCommand(
    SaveRoleMenuRequest Menu)
    : IRequest<Result<string>>;

#endregion

#region --- HANDLER ---

internal sealed class SaveRoleMenuCommandHandler(
    QubeFinDataContext context)
    : IRequestHandler<SaveRoleMenuCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        SaveRoleMenuCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================================
        // VALIDATE MENU
        // =========================================================

        var menu = await context.TblMenus
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.Menu.MenuId,
                cancellationToken);

        if (menu is null)
        {
            return Result.Fail("Menu not found.");
        }


        // =========================================================
        // GET MENU PERMISSIONS
        // =========================================================
        //
        // Get all permissions configured for this menu.
        //
        // Tbl_MenuPermission
        //
        // These IDs will be stored in:
        //
        // Tbl_RoleMenuPermission.MenuPermissionId
        // Tbl_UserMenuPermission.MenuPermissionId
        //
        // =========================================================

        var menuPermissionIds = await context.TblMenuPermissions
            .Where(x => x.MenuId == request.Menu.MenuId)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var menuPermissionIdSet =
            menuPermissionIds.ToHashSet();


        // =========================================================
        // VALIDATE ROLES
        // =========================================================

        var roleIds = request.Menu.RoleIds
            .Distinct()
            .ToList();

        var validRoleIds = await context.TblRoles
            .Where(x =>
                roleIds.Contains(x.Id) &&
                x.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validRoleIds.Count != roleIds.Count)
        {
            return Result.Fail(
                "One or more selected roles are invalid or inactive.");
        }


        // =========================================================
        // VALIDATE USERS
        // =========================================================

        var userIds = request.Menu.UserIds
            .Distinct()
            .ToList();

        var validUserIds = await context.TblUsers
            .Where(x =>
                userIds.Contains(x.Id) &&
                x.IsActive &&
                x.EmployeeId != null)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (validUserIds.Count != userIds.Count)
        {
            return Result.Fail(
                "One or more selected users are invalid, inactive, or not linked to an employee.");
        }


        // =========================================================
        // TRANSACTION
        // =========================================================

        await using var transaction =
            await context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            // =====================================================
            // REMOVE EXISTING ROLE MENU PERMISSIONS
            // =====================================================
            //
            // Remove all existing role permissions for this menu.
            //
            // We identify the menu through Tbl_MenuPermission.
            //
            // =====================================================

            var existingRolePermissions =
                await context.TblRoleMenuPermissions
                    .Where(x =>
                        menuPermissionIdSet.Contains(
                            x.MenuPermissionId))
                    .ToListAsync(cancellationToken);

            if (existingRolePermissions.Count > 0)
            {
                context.TblRoleMenuPermissions.RemoveRange(
                    existingRolePermissions);
            }


            // =====================================================
            // ADD ROLE MENU PERMISSIONS
            // =====================================================
            //
            // Every selected role receives every permission
            // configured for this menu.
            //
            // Example:
            //
            // Menu:
            // Employee
            //
            // Permissions:
            // VIEW
            // ADD
            // EDIT
            //
            // RoleIds:
            // Admin
            // HR
            //
            // Result:
            //
            // Admin -> VIEW
            // Admin -> ADD
            // Admin -> EDIT
            // HR    -> VIEW
            // HR    -> ADD
            // HR    -> EDIT
            //
            // =====================================================

            var newRolePermissions = roleIds
                .SelectMany(roleId =>
                    menuPermissionIds.Select(
                        menuPermissionId =>
                            new TblRoleMenuPermission
                            {
                                RoleId = roleId,

                                MenuPermissionId =
                                    menuPermissionId,

                                AccessClaimToken =
                                    string.Empty,

                                CreatedOn =
                                    DateTime.UtcNow,

                                CreatedBy =
                                    Guid.Empty
                                // TODO:
                                // Replace with current user ID
                            }))
                .ToList();

            if (newRolePermissions.Count > 0)
            {
                await context.TblRoleMenuPermissions
                    .AddRangeAsync(
                        newRolePermissions,
                        cancellationToken);
            }


            // =====================================================
            // REMOVE EXISTING USER MENU PERMISSIONS
            // =====================================================
            //
            // Remove all existing user permissions for this menu.
            //
            // =====================================================

            var existingUserPermissions =
                await context.TblUserMenuPermissions
                    .Where(x =>
                        menuPermissionIdSet.Contains(
                            x.MenuPermissionId))
                    .ToListAsync(cancellationToken);

            if (existingUserPermissions.Count > 0)
            {
                context.TblUserMenuPermissions.RemoveRange(
                    existingUserPermissions);
            }


            // =====================================================
            // ADD USER MENU PERMISSIONS
            // =====================================================
            //
            // Every selected user receives every permission
            // configured for this menu.
            //
            // =====================================================

            var newUserPermissions = userIds
                .SelectMany(userId =>
                    menuPermissionIds.Select(
                        menuPermissionId =>
                            new TblUserMenuPermission
                            {
                                UserId = userId,

                                MenuPermissionId =
                                    menuPermissionId,

                                AccessClaimToken =
                                    string.Empty,

                                CreatedOn =
                                    DateTime.UtcNow,

                                CreatedBy =
                                    Guid.Empty
                                // TODO:
                                // Replace with current user ID
                            }))
                .ToList();

            if (newUserPermissions.Count > 0)
            {
                await context.TblUserMenuPermissions
                    .AddRangeAsync(
                        newUserPermissions,
                        cancellationToken);
            }


            // =====================================================
            // SAVE
            // =====================================================

            await context.SaveChangesAsync(
                cancellationToken);


            // =====================================================
            // COMMIT
            // =====================================================

            await transaction.CommitAsync(
                cancellationToken);


            return Result.Ok(
                "Role and user menu permissions saved successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(
                cancellationToken);

            return Result.Fail(
                $"Failed to save menu permissions. {ex.Message}");
        }
    }
}

#endregion