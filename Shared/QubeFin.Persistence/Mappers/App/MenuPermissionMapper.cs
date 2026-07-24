using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblMenuPermission;

namespace QubeFin.Persistence.Mappers.App;

public static class MenuPermissionMapper
{
    public static MenuPermission ToDomain(this Entity entity)
    {
        return new MenuPermission(
            entity.Id,
            entity.MenuId,
            entity.PermissionToken,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn);
    }

    public static Entity ToEntity(this MenuPermission domain)
    {
        return new Entity
        {
            Id = domain.Id,
            MenuId = domain.MenuId,
            PermissionToken = domain.PermissionToken,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}

