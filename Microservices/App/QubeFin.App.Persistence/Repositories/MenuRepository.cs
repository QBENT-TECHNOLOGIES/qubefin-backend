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
    void Update(Menu menu);
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

    public void Update(Menu menu)
    {
        throw new NotImplementedException();
    }
}
