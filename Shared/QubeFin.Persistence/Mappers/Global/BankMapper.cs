using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblFinancialInstitute;

namespace QubeFin.Persistence.Mappers.Global;

public static class BankMapper
{
    public static Bank ToDomain(this Entity entity)
    {
        return new Bank(
            entity.Id,
            entity.Name,
            entity.IsBank,
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
            IsBank = domain.IsBank,
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
