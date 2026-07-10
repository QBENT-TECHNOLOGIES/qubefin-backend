using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{   
    public interface IOrganizationUnitRepository
    {
        Task CreateAsync(OrganizationUnit organizationUnit);
        Task UpdateAsync(OrganizationUnit organizationUnit);
        Task<IEnumerable<OrganizationUnit>> GetAllAsync();
        Task<OrganizationUnit?> GetByIdAsync(Guid id);
    }
    public class OrganizationUnitRepository(QubeFinDataContext context) : IOrganizationUnitRepository
    {
        public async Task CreateAsync(OrganizationUnit organizationUnit)
        {
            await context.TblOrganizationUnits.AddAsync(organizationUnit.ToEntity());
        }
        public Task UpdateAsync(OrganizationUnit organizationUnit)
        {
            context.TblOrganizationUnits.Update(organizationUnit.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<OrganizationUnit>> GetAllAsync()
        {
            var entity = await context.TblOrganizationUnits.Include(m => m.OrganizationUnitType).Include(m => m.CreatedByNavigation)
                .ThenInclude(m => m.Employee).Include(m => m.LastModifiedByNavigation).ThenInclude(m => m.Employee).AsNoTracking().ToListAsync();
            return entity.Select(m => m.ToDomain());
        }
        public async Task<OrganizationUnit?> GetByIdAsync(Guid id)
        {
            var entity = await context.TblOrganizationUnits.Include(m => m.OrganizationUnitType).Include(m => m.CreatedByNavigation)
                .ThenInclude(m => m.Employee).Include(m => m.LastModifiedByNavigation).ThenInclude(m => m.Employee).AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return entity?.ToDomain();
        }
    }
}
