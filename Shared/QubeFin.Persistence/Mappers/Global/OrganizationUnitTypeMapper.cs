using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = QubeFin.Persistence.Entities.TblOrganizationUnitType;
namespace QubeFin.Persistence.Mappers.Global
{
    public static class OrganizationUnitTypeMapper
    {
        public static OrganizationUnitType ToDomain(this Entity entity)
        {
            return new OrganizationUnitType(
                entity.Id,
                entity.Name,
                entity.LevelNo
            );
        }
    }
}
