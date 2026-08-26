using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Persistence.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetRolesAsync();
    Task<Role?> GetByIdAsync(Guid id);
    void AddAsync(Role role);
    void Update(Role role);
    void Remove(Role role);
}

public class RoleRepository(QubeFinDataContext context) : IRoleRepository
{
    public void AddAsync(Role role)
    {
        context.TblRoles.Add(role.ToEntity());
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        var role = await context.TblRoles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return role?.ToDomain();
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        var roles = await context.TblRoles.AsNoTracking().OrderBy(r => r.Name).Select(r => r.ToDomain()).ToListAsync();
        return roles;
    }

    public void Remove(Role role)
    {
        throw new NotImplementedException();
    }

    public void Update(Role role)
    {
        context.TblRoles.Update(role.ToEntity());
    }
}
