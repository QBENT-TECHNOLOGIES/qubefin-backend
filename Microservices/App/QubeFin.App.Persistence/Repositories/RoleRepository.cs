using QubeFin.Persistence.Models.App;

namespace QubeFin.App.Persistence.Repositories;

public interface IRoleRepository
{
    Task<List<Role>> GetRolesAsync();
    Task<Role?> GetByIdAsync(Guid id);
    Task AddAsync(Role role);
    void Update(Role role);
    void Remove(Role role);
}

public class RoleRepository : IRoleRepository
{
    public Task AddAsync(Role role)
    {
        throw new NotImplementedException();
    }

    public Task<Role?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Role>> GetRolesAsync()
    {
        throw new NotImplementedException();
    }

    public void Remove(Role role)
    {
        throw new NotImplementedException();
    }

    public void Update(Role role)
    {
        throw new NotImplementedException();
    }
}
