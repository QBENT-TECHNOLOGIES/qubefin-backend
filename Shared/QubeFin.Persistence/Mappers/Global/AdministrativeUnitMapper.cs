using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblAdministrativeUnit;

namespace QubeFin.Persistence.Mappers.Global;

public static class AdministrativeUnitMapper
{
    public static AdministrativeUnit ToDomain(this Entity entity)
    {
        return new AdministrativeUnit(
            entity.Id,
            entity.AdministrativeUnitTypeId,
            entity.Name,
            entity.ParentId,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn);
    }

    public static Entity ToEntity(this AdministrativeUnit domain)
    {
        return new Entity
        {
            Id = domain.Id,
            AdministrativeUnitTypeId = domain.AdministrativeUnitTypeId,
            Name = domain.Name,
            ParentId = domain.ParentId,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}
