using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Persistence.Repositories
{   
    public interface IOrganizationUnitTypeRepository
    {
        Task<IEnumerable<OrganizationUnitType>> GetAllAsync();
    }
    public class OrganizationUnitTypeRepository(QubeFinDataContext context) : IOrganizationUnitTypeRepository
    {
        public async Task<IEnumerable<OrganizationUnitType>> GetAllAsync()
        {
            var entities = await context.TblOrganizationUnitTypes.AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
    }
}
