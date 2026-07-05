using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblMenu;

namespace QubeFin.Persistence.Mappers.App;

public static class MenuMapper
{
    public static Menu ToDomain(this Entity entity)
    {
        return new Menu(
            entity.Id,
            entity.Name,
            entity.Icon,
            entity.Target,
            entity.ParentId,
            entity.DisplayPosition,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn);
    }

    public static Entity ToEntity(this Menu domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Name = domain.Name,
            Icon = domain.Icon,
            Target = domain.Target,
            ParentId = domain.ParentId,
            DisplayPosition = domain.DisplayPosition,
            IsActive = domain.IsActive,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}
