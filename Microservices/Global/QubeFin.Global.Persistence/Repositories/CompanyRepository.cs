using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Persistence.Repositories
{   
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompanies();
    }
    public class CompanyRepository(QubeFinDataContext context): ICompanyRepository
    {
        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            var entities = await context.TblCompanies.AsNoTracking().ToListAsync();
            return entities.Select(m => m.ToDomain());
        }
    }
}
