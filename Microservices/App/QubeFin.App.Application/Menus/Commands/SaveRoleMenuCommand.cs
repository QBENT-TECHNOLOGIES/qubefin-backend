using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Application.Menus.Models;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Application.Menus.Commands;

#region --- COMMAND ---

public sealed record SaveRoleMenuCommand(SaveRoleMenuRequest Menu, Guid userId) : IRequest<Result<string>>;

#endregion

#region --- HANDLER ---

internal sealed class SaveRoleMenuCommandHandler(QubeFinDataContext context, IUnitOfWork unitOfWork) : IRequestHandler<SaveRoleMenuCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveRoleMenuCommand request, CancellationToken cancellationToken)
    {

        var menu = await context.TblMenus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Menu.MenuId, cancellationToken);

        if (menu is null)
        {
            return Result.Fail("Menu not found.");
        }

        var menuPermissionIds = await context.TblMenuPermissions.Where(x => x.MenuId == request.Menu.MenuId).Select(x => x.Id).ToListAsync(cancellationToken);
        var menuPermissionIdSet = menuPermissionIds.ToHashSet();

        var roleIds = request.Menu.Roles.Select(x => x.RoleId).Distinct().ToList();
        var validRoleIds = await context.TblRoles.Where(x => roleIds.Contains(x.Id) && x.IsActive).Select(x => x.Id).ToListAsync(cancellationToken);

        if (validRoleIds.Count != roleIds.Count)
        {
            return Result.Fail("One or more selected roles are invalid or inactive.");
        }

        var userIds = request.Menu.Users.Select(x => x.UserId).Distinct().ToList();
        var validUserIds = await context.TblUsers.Where(x => userIds.Contains(x.Id) && x.IsActive && x.EmployeeId != null).Select(x => x.Id).ToListAsync(cancellationToken);

        if (validUserIds.Count != userIds.Count)
        {
            return Result.Fail("One or more selected users are invalid, inactive, or not linked to an employee.");
        }

        var submittedRolePermissionIds = request.Menu.Roles.SelectMany(x => x.MenuPermissionIds).Distinct().ToList();
        var invalidRolePermissionIds = submittedRolePermissionIds.Where(x => !menuPermissionIdSet.Contains(x)).ToList();

        if (invalidRolePermissionIds.Count > 0)
        {
            return Result.Fail("One or more role permissions do not belong to this menu.");
        }


        var submittedUserPermissionIds = request.Menu.Users.SelectMany(x => x.MenuPermissionIds).Distinct().ToList();
        var invalidUserPermissionIds = submittedUserPermissionIds.Where(x => !menuPermissionIdSet.Contains(x)).ToList();

        if (invalidUserPermissionIds.Count > 0)
        {
            return Result.Fail("One or more user permissions do not belong to this menu.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {

            var existingRolePermissions = await context.TblRoleMenuPermissions.Where(x => menuPermissionIdSet.Contains(x.MenuPermissionId)).ToListAsync(cancellationToken);

            if (existingRolePermissions.Count > 0)
            {
                context.TblRoleMenuPermissions.RemoveRange(existingRolePermissions);
            }


            var newRolePermissions = request.Menu.Roles.SelectMany(role => role.MenuPermissionIds.Distinct()
            .Select(menuPermissionId => new TblRoleMenuPermission
            {
                RoleId = role.RoleId,
                MenuPermissionId = menuPermissionId,
                AccessClaimToken = string.Empty,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.userId
            })).ToList();

            if (newRolePermissions.Count > 0)
            {
                await context.TblRoleMenuPermissions.AddRangeAsync(newRolePermissions, cancellationToken);
            }

            var existingUserPermissions = await context.TblUserMenuPermissions.Where(x => menuPermissionIdSet.Contains(x.MenuPermissionId)).ToListAsync(cancellationToken);

            if (existingUserPermissions.Count > 0)
            {
                context.TblUserMenuPermissions.RemoveRange(existingUserPermissions);
            }

            var newUserPermissions = request.Menu.Users.SelectMany(user => user.MenuPermissionIds.Distinct()
            .Select(menuPermissionId => new TblUserMenuPermission
            {
                UserId = user.UserId,
                MenuPermissionId = menuPermissionId,
                AccessClaimToken = string.Empty,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.userId
            })).ToList();

            if (newUserPermissions.Count > 0)
            {
                await context.TblUserMenuPermissions.AddRangeAsync(newUserPermissions, cancellationToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);


            return Result.Ok("Role and user menu permissions saved successfully.");
        }
        catch (Exception ex)
        {

            await transaction.RollbackAsync(cancellationToken);

            return Result.Fail($"Failed to save menu permissions. {ex.Message}");
        }
    }
}

#endregion