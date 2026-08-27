using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IDepartmentRepository
{
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task<Department?> GetByIdAsync(Guid id);
    Task<IEnumerable<Department>> GetAllAsync();
    Task<bool> ExistsAsync(string name);
}
public class DepartmentRepository(QubeFinDataContext context) : IDepartmentRepository
{
    public async Task AddAsync(Department department)
    {
        await context.TblDepartments.AddAsync(department.ToEntity());
    }
    public Task UpdateAsync(Department department)
    {
        context.TblDepartments.Update(department.ToEntity());
        return Task.CompletedTask;
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblDepartments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return entity?.ToDomain();
    }
    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        var entities = await context.TblDepartments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
        return entities.Select(x => x.ToDomain());
    }
    public Task<bool> ExistsAsync(string name)
    {
        return context.TblDepartments
            .AsNoTracking()
            .AnyAsync(x => x.Name.ToLower() == name.ToLower());
    }
}

