using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IOrganizationUnitRepository
{
    Task<IEnumerable<OrganizationUnitTree>> GetAllAsync(CancellationToken cancellationToken);
    Task<OrganizationUnit?> GetByIdAsync(Guid id);
    Task AddAsync(OrganizationUnit organizationUnit);
    void Update(OrganizationUnit organizationUnit);
}

internal class OrganizationUnitRepository(QubeFinDataContext context) : IOrganizationUnitRepository
{
    public async Task AddAsync(OrganizationUnit organizationUnit)
    {
        await context.TblOrganizationUnits.AddAsync(organizationUnit.ToEntity());
    }

    public async Task<IEnumerable<OrganizationUnitTree>> GetAllAsync(CancellationToken cancellationToken)
    {
        var organizationUnitEntities = await context
            .TblOrganizationUnits
            .Include(m => m.OrganizationUnitType)
            .AsNoTracking()
            .Select(m => new OrganizationUnitTree
            {
                Id = m.Id,
                OrganizationUnitTypeId = m.OrganizationUnitTypeId,
                OrganizationUnitTypeIcon = m.OrganizationUnitType.Icon,
                OrganizationUnitTypeName = m.OrganizationUnitType.Name,
                Name = m.Name,
                ParentId = m.ParentId
            })
            .ToListAsync(cancellationToken);

        return organizationUnitEntities;
    }
    public async Task<OrganizationUnit?> GetByIdAsync(Guid id)
    {
        var organizationUnitEntity = await context.TblOrganizationUnits.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return organizationUnitEntity?.ToDomain();
    }

    public void Update(OrganizationUnit organizationUnit)
    {
        context.TblOrganizationUnits.Update(organizationUnit.ToEntity());
    }
}
