using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblOrganizationUnit;

namespace QubeFin.Persistence.Mappers.Global;

public static class OrganizationUnitMapper
{
    public static OrganizationUnit ToDomain(this Entity entity)
    {
        return new OrganizationUnit(
            entity.Id,
            entity.OrganizationUnitTypeId,
            entity.Name,
            entity.CodeVal,
            entity.AttendanceInTime,
            entity.AttendanceOutTime,
            entity.ParentId,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn
        );
    }

    public static Entity ToEntity(this OrganizationUnit domain)
    {
        return new Entity
        {
            Id = domain.Id,
            OrganizationUnitTypeId = domain.OrganizationUnitTypeId,
            Name = domain.Name,
            CodeVal = domain.CodeVal,
            ParentId = domain.ParentId,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn,
            AttendanceInTime = domain.AttendanceInTime,
            AttendanceOutTime = domain.AttendanceOutTime
        };
    }

    public static IEnumerable<OrganizationUnit> ToDomain(this IEnumerable<Entity> entities)
    {
        return entities.Select(e => e.ToDomain());
    }

    public static IEnumerable<Entity> ToEntity(this IEnumerable<OrganizationUnit> domains)
    {
        return domains.Select(d => d.ToEntity());
    }
}
