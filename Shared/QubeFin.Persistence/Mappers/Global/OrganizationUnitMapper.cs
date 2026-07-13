using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entity = QubeFin.Persistence.Entities.TblOrganizationUnit;

namespace QubeFin.Persistence.Mappers.Global;

public static class OrganizationUnitMapper
{
    public static OrganizationUnit ToDomain(this Entity entity)
    {
        // Use the domain factory method that exists instead of a non-existent 12-arg ctor
        var domain = OrganizationUnit.Create(
            entity.Id,
            entity.OrganizationUnitTypeId,
            entity.Name,
            entity.CodeVal,
            entity.ParentId,
            entity.CreatedBy,
            entity.AttendanceInTime,
            entity.AttendanceOutTime
        );

        // Preserve audit timestamps/ids if domain exposes setters
        domain.CreatedOn = entity.CreatedOn;
        domain.LastModifiedOn = entity.LastModifiedOn;
        domain.LastModifiedBy = entity.LastModifiedBy;

        return domain;
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
