using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Persistence.Repositories
{   
    public interface ISalaryComponentRepository
    {
        Task CreateSalaryComponent(SalaryComponent salaryComponent);
        Task UpdateSalaryComponent(SalaryComponent salaryComponent);
        Task <SalaryComponent?> GetSalaryComponentById(Guid id);
        Task <IEnumerable<SalaryComponent>> GetAllSalaryComponents();
        Task<bool> ExistsByNameAsync(string name, Guid categoryId, Guid? excludeId = null);
    }
    public class SalaryComponentRepository(QubeFinDataContext context) : ISalaryComponentRepository
    {
        public async Task CreateSalaryComponent(SalaryComponent salaryComponent)
        {
            await context.TblSalaryComponents.AddAsync(salaryComponent.ToEntity());
        }
        public Task UpdateSalaryComponent(SalaryComponent salaryComponent)
        {
            context.TblSalaryComponents.Update(salaryComponent.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<SalaryComponent?> GetSalaryComponentById(Guid id)
        {
            var entity = await context.TblSalaryComponents.Include(m => m.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return entity?.ToDomain();
        }
        public async Task<IEnumerable<SalaryComponent>> GetAllSalaryComponents()
        {
            var entities = await context.TblSalaryComponents.Include(m => m.Category).AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
        public async Task<bool> ExistsByNameAsync(string name, Guid categoryId, Guid? excludeId = null)
        {
            return await context.TblSalaryComponents
                .AsNoTracking()
                .AnyAsync(x => x.CategoryId == categoryId
                    && x.Name.ToLower() == name.ToLower()
                    && (!excludeId.HasValue || x.Id != excludeId.Value));
        }
    }
}
