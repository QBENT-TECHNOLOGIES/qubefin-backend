using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = QubeFin.Persistence.Entities.TblSalaryComponentCategory;


namespace QubeFin.Persistence.Mappers.Hrms
{
    public static class SalaryComponentCategoryMapper
    {
        public static SalaryComponentCategory ToDomain(this Entity entity)
        {
            return new SalaryComponentCategory(
                entity.Id,
                entity.Name
            );
        }
    }
        
}
