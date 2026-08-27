using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Menus.Queries;

#region --- QUERY ---
public record GetParentMenusByUserQuery(Guid EmployeeId) : IRequest<Result<List<GetParentMenusByUserResponse>>>;
#endregion

#region ---RESPONSE ---
public record GetParentMenusByUserResponse(Guid Id, string Name, string Icon);
#endregion

#region --- HANDLER ---
internal sealed class GetParentMenusByUserHandler(QubeFinDataContext context)
    : IRequestHandler<GetParentMenusByUserQuery, Result<List<GetParentMenusByUserResponse>>>
{
    public async Task<Result<List<GetParentMenusByUserResponse>>> Handle(GetParentMenusByUserQuery request, CancellationToken cancellationToken)
    {
        var designationId = await context.TblEmployeeDesignations
            .AsNoTracking()
            .Where(e => e.EmployeeId == request.EmployeeId)
            .OrderByDescending(d => d.EffectiveTo == null)
            .ThenByDescending(d => d.EffectiveFrom)
            .Select(d => (Guid?)d.DesignationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (designationId is null)
        {
            return Result.Ok(new List<GetParentMenusByUserResponse>());
        }

        var roleId = await context.TblDesignationRoles
            .AsNoTracking()
            .Where(dr => dr.DesignationId == designationId)
            .OrderByDescending(dr => dr.CreatedOn)
            .Select(dr => (Guid?)dr.RoleId)
            .FirstOrDefaultAsync(cancellationToken);

        if (roleId is null)
        {
            return Result.Ok(new List<GetParentMenusByUserResponse>());
        }

        var menus = await context.TblRoleMenuPermissions
            .AsNoTracking()
            .Where(rmp => rmp.RoleId == roleId && rmp.MenuPermission.Menu.ParentId == null && rmp.MenuPermission.Menu.IsActive)
            .OrderBy(rmp => rmp.MenuPermission.Menu.DisplayPosition)
            .Select(rmp => new GetParentMenusByUserResponse(
                rmp.MenuPermission.Menu.Id,
                rmp.MenuPermission.Menu.Name,
                rmp.MenuPermission.Menu.Icon))
            .Distinct()
            .ToListAsync(cancellationToken);

        return Result.Ok(menus);
    }
}
#endregion