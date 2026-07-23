using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblPermission;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Mappers.App;

public static class PermissionMapper
{
    public static Permission ToDomain(this Entity entity)
    {
        return new Permission(
            entity.PermissionToken,
            entity.DisplayPosition
            );
    }

    public static Entity ToEntity(this Permission domain)
    {
        return new Entity
        {
            PermissionToken = domain.PermissionToken,
            DisplayPosition = domain.DisplayPosition
        };
    }

    public static IEnumerable<Permission> ToDomain(this IEnumerable<Entity> entities)
    {
        return entities.Select(e => e.ToDomain());
    }

    public static IEnumerable<Entity> ToEntity(this IEnumerable<Permission> domains)
    {
        return domains.Select(d => d.ToEntity());
    }
}
