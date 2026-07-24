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

    public Task<int> GetMaxMenuPositionAsync()
    {
        throw new NotImplementedException();
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
            if (!menu.Permissions.Any(x => x.PermissionToken == existing.PermissionToken))
            {
                context.TblMenuPermissions.Remove(existing);
            }
        }

        foreach (var permission in menu.Permissions)
        {
            var existing = entity.TblMenuPermissions.FirstOrDefault(x => x.PermissionToken == permission.PermissionToken);

            if (existing == null)
            {
                // New assignment
                var newMenuPermission = MenuPermission.Create(entity.Id, permission.PermissionToken, userId);
                entity.TblMenuPermissions.Add(newMenuPermission.ToEntity());
            }
            else
            {
                // Existing assignment
                existing.PermissionToken = permission.PermissionToken;
                existing.LastModifiedBy = menu.LastModifiedBy;
                existing.LastModifiedOn = DateTime.Now;
            }
        }
    }
}
