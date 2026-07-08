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
            var entity = await context.TblSalaryComponents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return entity?.ToDomain();
        }
        public async Task<IEnumerable<SalaryComponent>> GetAllSalaryComponents()
        {
            var entities = await context.TblSalaryComponents.AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
    }
}
