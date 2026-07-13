using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;
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
                entity.ParentId,
                entity.CreatedOn,
                entity.CreatedBy,
            entity.CreatedOn,
                entity.LastModifiedBy,
            entity.LastModifiedOn,
                entity.AttendanceInTime,
                entity.AttendanceOutTime
            );
            string cFullName = string.Join(" ", new[]
            {
                entity.CreatedByNavigation?.Employee?.FirstName,
                entity.CreatedByNavigation?.Employee?.MiddleName,
                entity.CreatedByNavigation?.Employee?.LastName
            }.Where(s => !string.IsNullOrWhiteSpace(s)));
            string lFullName = string.Join(" ", new[]
           {
                entity.LastModifiedByNavigation?.Employee?.FirstName,
                entity.LastModifiedByNavigation?.Employee?.MiddleName,
                entity.LastModifiedByNavigation?.Employee?.LastName
            }.Where(s => !string.IsNullOrWhiteSpace(s)));
            domain.SetTypeAndNames(entity.OrganizationUnitType?.Name ?? string.Empty, cFullName, lFullName);
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
