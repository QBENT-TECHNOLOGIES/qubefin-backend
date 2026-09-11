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
    public async Task UpdateAsync(Department department)
    {
        var existingEntity = await context.TblDepartments.FindAsync(department.Id);

        if (existingEntity != null)
        {
            existingEntity.Name = department.Name;
            existingEntity.HodEmployeeId = department.HodEmployeeId == Guid.Empty ? null : department.HodEmployeeId;
            existingEntity.IsActive = department.IsActive;
            existingEntity.LastModifiedOn = department.LastModifiedOn;
            existingEntity.LastModifiedBy = department.LastModifiedBy;

            context.TblDepartments.Update(existingEntity);
        }
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblDepartments.Include(m=>m.HodEmployee)
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

