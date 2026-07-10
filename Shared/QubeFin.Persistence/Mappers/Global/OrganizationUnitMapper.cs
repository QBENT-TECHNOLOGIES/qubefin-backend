using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = QubeFin.Persistence.Entities.TblOrganizationUnit;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class OrganizationUnitMapper
    {
        public static OrganizationUnit ToDomain(this Entity entity)
        {
            var domain = new OrganizationUnit(
                entity.Id,
                entity.OrganizationUnitTypeId,
                entity.Name,
                entity.CodeVal,
                entity.ParentId,
                entity.CreatedOn,
                entity.CreatedBy,
                entity.LastModifiedOn,
                entity.LastModifiedBy,
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
        public static Entity ToEntity(this OrganizationUnit organizationUnit)
        {
            return new Entity
            {
                Id = organizationUnit.Id,
                OrganizationUnitTypeId = organizationUnit.OrganizationUnitTypeId,
                Name = organizationUnit.Name,
                CodeVal = organizationUnit.CodeVal,
                ParentId = organizationUnit.ParentId,
                CreatedOn = organizationUnit.CreatedOn,
                CreatedBy = organizationUnit.CreatedBy,
                LastModifiedOn = organizationUnit.LastModifiedOn,
                LastModifiedBy = organizationUnit.LastModifiedBy,
                AttendanceInTime = organizationUnit.AttendanceInTime,
                AttendanceOutTime = organizationUnit.AttendanceOutTime,
            };
        }
    }
}
