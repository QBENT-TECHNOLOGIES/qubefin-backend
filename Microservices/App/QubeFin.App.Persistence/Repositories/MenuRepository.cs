using Azure.Core;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.App.Persistence.Repositories;

public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(Guid id);
    Task<int> GetMaxMenuPositionAsync();
    Task<IEnumerable<MenuTree>> GetTreeAsync();
    Task<IEnumerable<MenuTree>> GetTreeAsync(Guid employeeId);
    Task AddAsync(Menu menu);
    Task UpdateAsync(Menu menu, Guid userId);
    void Remove(Menu menu);
}

public class MenuRepository(QubeFinDataContext context) : IMenuRepository
{
    public async Task AddAsync(Menu menu)
    {
        await context.TblMenus.AddAsync(menu.ToEntity());
    }

    public async Task<Menu?> GetByIdAsync(Guid id)
    {
        var menu = await context
            .TblMenus
            .AsNoTracking()
            .Select(m => new Menu
            (
                m.Id, m.Name, m.Icon, m.Target, m.ParentId, m.DisplayPosition, m.IsActive, m.CreatedBy, m.CreatedOn, m.LastModifiedBy, m.LastModifiedOn
            ))
            .FirstOrDefaultAsync();

        return menu;
    }

    public async Task<int> GetMaxMenuPositionAsync()
    {
        var lastPosition = await context.TblMenus.MaxAsync(m => m.DisplayPosition);
        return lastPosition == 0 ? 0 : lastPosition;
    }

    public async Task<IEnumerable<MenuTree>> GetTreeAsync()
    {
        var menuTree = await context
           .TblMenus
           .AsNoTracking()
           .Select(m => new MenuTree
           {
               Id = m.Id,
               Name = m.Name,
               Icon = m.Icon,
               Target = m.Target,
               ParentId = m.ParentId,
               IsActive = m.IsActive,
               DisplayPosition = m.DisplayPosition,
           })
           .ToListAsync();

        return menuTree;
    }

    public async Task<IEnumerable<MenuTree>> GetTreeAsync(Guid employeeId)
    {
        // =========================================================
        // GET USER
        // =========================================================

        var userId = await context.TblUsers
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.IsActive)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();

        if (userId is null)
        {
            return Enumerable.Empty<MenuTree>();
        }


        // =========================================================
        // GET EMPLOYEE DESIGNATION
        // =========================================================

        var designationId = await context.TblEmployeeDesignations
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.EffectiveTo == null)
            .ThenByDescending(x => x.EffectiveFrom)
            .Select(x => (Guid?)x.DesignationId)
            .FirstOrDefaultAsync();

        if (designationId is null)
        {
            return Enumerable.Empty<MenuTree>();
        }


        // =========================================================
        // GET ROLE(S) FOR DESIGNATION
        // =========================================================

        var roleIds = await context.TblDesignationRoles
            .AsNoTracking()
            .Where(x => x.DesignationId == designationId)
            .Select(x => x.RoleId)
            .Distinct()
            .ToListAsync();


        // =========================================================
        // GET ROLE BASED MENUS
        // =========================================================

        var roleMenuIds = await context.TblRoleMenuPermissions
            .AsNoTracking()
            .Where(x =>
                roleIds.Contains(x.RoleId) &&
                x.MenuPermission.Menu.IsActive)
            .Select(x => x.MenuPermission.Menu.Id)
            .Distinct()
            .ToListAsync();


        // =========================================================
        // GET USER SPECIFIC MENUS
        // =========================================================

        var userMenuIds = await context.TblUserMenuPermissions
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.MenuPermission.Menu.IsActive)
            .Select(x => x.MenuPermission.Menu.Id)
            .Distinct()
            .ToListAsync();


        // =========================================================
        // COMBINE ROLE + USER MENUS
        // =========================================================

        var menuIds = roleMenuIds
            .Concat(userMenuIds)
            .Distinct()
            .ToList();


        if (menuIds.Count == 0)
        {
            return Enumerable.Empty<MenuTree>();
        }


        // =========================================================
        // GET MENUS
        // =========================================================

        var menus = await context.TblMenus
            .AsNoTracking()
            .Where(x =>
                menuIds.Contains(x.Id) &&
                x.IsActive)
            .OrderBy(x => x.DisplayPosition)
            .Select(x => new MenuTree
            {
                Id = x.Id,
                Name = x.Name,
                Icon = x.Icon,
                Target = x.Target,
                ParentId = x.ParentId,
                DisplayPosition = x.DisplayPosition,
                IsActive = x.IsActive,
                Children = new List<MenuTree>()
            })
            .ToListAsync();


        return menus;
    }

    public void Remove(Menu menu)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Menu menu, Guid userId)
    {
        var entity = await context.TblMenus
                .Include(x => x.TblMenuPermissions)
                .FirstOrDefaultAsync(x => x.Id == menu.Id);

        if (entity == null)
            throw new Exception("Menu not found.");

        // Update parent
        entity.Name = menu.Name;
        entity.Icon = menu.Icon;
        entity.Target = menu.Target;
        entity.ParentId = menu.ParentId;
        //entity.DisplayPosition = menu.DisplayPosition;
        entity.LastModifiedBy = userId;
        entity.LastModifiedOn = DateTime.Now;

        // Remove deleted assignments
        foreach (var existing in entity.TblMenuPermissions.ToList())
        {
            if (!menu.Permissions.Any(x => x.Id == existing.PermissionId))
            {
                context.TblMenuPermissions.Remove(existing);
            }
        }

        foreach (var permission in menu.Permissions)
        {
            var existing = entity.TblMenuPermissions.FirstOrDefault(x => x.Id == permission.Id);

            if (existing == null)
            {
                // New assignment
                var newMenuPermission = MenuPermission.Create(entity.Id, permission.Id, userId);
                entity.TblMenuPermissions.Add(newMenuPermission.ToEntity());
            }
            else
            {
                // Existing assignment
                existing.PermissionId = permission.Id;
                existing.LastModifiedBy = menu.LastModifiedBy;
                existing.LastModifiedOn = DateTime.Now;
            }
        }
    }
}
