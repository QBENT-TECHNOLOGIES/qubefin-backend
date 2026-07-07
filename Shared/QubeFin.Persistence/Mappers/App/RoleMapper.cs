using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblRole;

namespace QubeFin.Persistence.Mappers.App;

public static class RoleMapper
{
    public static Role ToDomain(this Entity entity)
    {
        return new Role(
            entity.Id,
            entity.Name,
            entity.IsActive,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn);
    }

    public static Entity ToEntity(this Role domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Name = domain.Name,
            IsActive = domain.IsActive,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}
