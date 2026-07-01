using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IAdministrativeUnitRepository
{
    void Add(AdministrativeUnit administrativeUnit);
    Task<IEnumerable<AdministrativeUnitTree>> GetAllAsync(CancellationToken cancellationToken);
}

public class AdministrativeUnitRepository(QubeFinDataContext context) : IAdministrativeUnitRepository
{
    public void Add(AdministrativeUnit administrativeUnit)
    {
        context.TblAdministrativeUnits.Add(administrativeUnit.ToEntity());
    }

    public async Task<IEnumerable<AdministrativeUnitTree>> GetAllAsync(CancellationToken cancellationToken)
    {
        var administrativeUnitEntities = await context
            .TblAdministrativeUnits
            .Include(m => m.AdministrativeUnitType)
            .AsNoTracking()
            .Select(m => new AdministrativeUnitTree
            {
                Id = m.Id,
                AdministrativeUnitTypeId = m.AdministrativeUnitTypeId,
                AdministrativeUnitTypeIcon = m.AdministrativeUnitType.Icon,
                AdministrativeUnitTypeName = m.AdministrativeUnitType.Name,
                Name = m.Name,
                ParentId = m.ParentId,
                IsActive = m.IsActive
            })
            .ToListAsync(cancellationToken);

        return administrativeUnitEntities;
    }
}
