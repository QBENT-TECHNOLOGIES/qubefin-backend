using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IOrganizationUnitRepository
{
    void Add(OrganizationUnit organizationUnit);
    Task<IEnumerable<OrganizationUnitTree>> GetAllAsync(CancellationToken cancellationToken);
}

internal class OrganizationUnitRepository(QubeFinDataContext context) : IOrganizationUnitRepository
{
    public void Add(OrganizationUnit organizationUnit)
    {
        context.TblOrganizationUnits.Add(organizationUnit.ToEntity());
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
                OrganizationUnitTypeName = m.OrganizationUnitType.Name,
                Name = m.Name,
                ParentId = m.ParentId
            })
            .ToListAsync(cancellationToken);

        return organizationUnitEntities;
    }
}
