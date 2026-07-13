using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface ISalaryComponentCategoryRepository
    {
        Task<IEnumerable<SalaryComponentCategory>> GetAllSalaryComponentCategories();

    }
    public class SalaryComponentCategoryRepository(QubeFinDataContext context) : ISalaryComponentCategoryRepository
    {
        public async Task<IEnumerable<SalaryComponentCategory>> GetAllSalaryComponentCategories()
        {
            var entities = await context.TblSalaryComponentCategories.AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
    }
}
