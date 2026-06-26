using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IAdministrativeUnitRepository
{
    void Add(AdministrativeUnit administrativeUnit);
}

public class AdministrativeUnitRepository(QubeFinDataContext context) : IAdministrativeUnitRepository
{
    public void Add(AdministrativeUnit administrativeUnit)
    {
        context.TblAdministrativeUnits.Add(administrativeUnit.ToEntity());
    }
}
