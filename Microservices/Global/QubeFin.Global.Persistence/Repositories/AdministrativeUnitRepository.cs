using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IAdministrativeUnitRepository
{
    Task<IEnumerable<AdministrativeUnitTree>> GetAllAsync(CancellationToken cancellationToken);
    Task<AdministrativeUnit?> GetByIdAsync(Guid id);
    Task AddAsync(AdministrativeUnit administrativeUnit);
    void Update(AdministrativeUnit administrativeUnit);
}

public class AdministrativeUnitRepository(QubeFinDataContext context) : IAdministrativeUnitRepository
{
    public async Task AddAsync(AdministrativeUnit administrativeUnit)
    {
        await context.TblAdministrativeUnits.AddAsync(administrativeUnit.ToEntity());
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

    public async Task<AdministrativeUnit?> GetByIdAsync(Guid id)
    {
        var administrativeUnitEntity = await context.TblAdministrativeUnits.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return administrativeUnitEntity?.ToDomain();
    }

    public void Update(AdministrativeUnit administrativeUnit)
    {
        context.TblAdministrativeUnits.Update(administrativeUnit.ToEntity());
    }
}
