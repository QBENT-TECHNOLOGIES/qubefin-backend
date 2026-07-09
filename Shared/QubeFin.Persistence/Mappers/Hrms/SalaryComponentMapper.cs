using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;
using Entity = QubeFin.Persistence.Entities.TblSalaryComponent;

namespace QubeFin.Persistence.Mappers.Hrms
{
    public static class SalaryComponentMapper
    {
        public static SalaryComponent ToDomain(this Entity entity)
        {
            var domain =  new SalaryComponent(
                entity.Id,
                entity.Name,
                entity.Code,
                entity.CategoryId,
                entity.IsTaxable,
                entity.IsPfapplicable,
                entity.IsEsiapplicable,
                entity.IsCtccomponent,
                entity.IsActive,
                entity.DisplayOrder,
                entity.CreatedOn,
                entity.CreatedBy,
                entity.LastModifiedOn,
                entity.LastModifiedBy
            );
            domain.SetCategoryName(entity.Category.Name);
            return domain;
        }
        public static Entity ToEntity(this SalaryComponent domain)
        {
            return new Entity
            {
                Id = domain.Id,
                Name = domain.Name,
                Code = domain.Code,
                CategoryId = domain.CategoryId,
                IsTaxable = domain.IsTaxable,
                IsPfapplicable = domain.IsPfapplicable,
                IsEsiapplicable = domain.IsEsiapplicable,
                IsCtccomponent = domain.IsCtccomponent,
                IsActive = domain.IsActive,
                DisplayOrder = domain.DisplayOrder,
                CreatedOn = domain.CreatedOn,
                CreatedBy = domain.CreatedBy,
                LastModifiedOn = domain.LastModifiedOn,
                LastModifiedBy = domain.LastModifiedBy
            };
        }
    }
}
