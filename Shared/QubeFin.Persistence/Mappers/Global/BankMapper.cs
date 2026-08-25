using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblBank;

namespace QubeFin.Persistence.Mappers.Global;

public static class BankMapper
{
    public static Bank ToDomain(this Entity entity)
    {
        return new Bank(
            entity.Id,
            entity.Name,
            entity.Alias,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn
        );
    }

    public static Entity ToEntity(this Bank domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Name = domain.Name,
            Alias = domain.Alias,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }

    public static IEnumerable<Bank> ToDomain(this IEnumerable<Entity> entities)
    {
        return entities.Select(e => e.ToDomain());
    }

    public static IEnumerable<Entity> ToEntity(this IEnumerable<Bank> domains)
    {
        return domains.Select(d => d.ToEntity());
    }
}
