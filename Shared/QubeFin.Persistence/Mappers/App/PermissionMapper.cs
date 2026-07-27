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
            entity.Id,
            entity.PermissionToken,
            entity.Description,
            entity.Icon,
            entity.BackgroundClass,
            entity.IconClass,
            entity.DisplayPosition
            );
    }

    public static Entity ToEntity(this Permission domain)
    {
        return new Entity
        {
            Id = domain.Id,
            PermissionToken = domain.PermissionToken,
            Description = domain.Description,
            Icon = domain.Icon,
            BackgroundClass = domain.BackgroundClass,
            IconClass = domain.IconClass,
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
