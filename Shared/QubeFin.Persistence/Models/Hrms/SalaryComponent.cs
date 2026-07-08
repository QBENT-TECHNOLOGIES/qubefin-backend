using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class SalaryComponent
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public bool IsTaxable { get; set; }

        public bool IsPfapplicable { get; set; }

        public bool IsEsiapplicable { get; set; }

        public bool IsCtccomponent { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public Guid? LastModifiedBy { get; set; }
        private SalaryComponent() { }
        public SalaryComponent(
            Guid id, 
            string name, 
            string code, 
            Guid categoryId, 
            bool isTaxable, 
            bool isPfapplicable, 
            bool isEsiapplicable, 
            bool isCtccomponent, 
            bool isActive, 
            int displayOrder, 
            DateTime createdOn, 
            Guid createdBy, 
            DateTime? lastModifiedOn, 
            Guid? lastModifiedBy)
        {
            Id = id;
            Name = name;
            Code = code;
            CategoryId = categoryId;
            IsTaxable = isTaxable;
            IsPfapplicable = isPfapplicable;
            IsEsiapplicable = isEsiapplicable;
            IsCtccomponent = isCtccomponent;
            IsActive = isActive;
            DisplayOrder = displayOrder;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            LastModifiedOn = lastModifiedOn;
            LastModifiedBy = lastModifiedBy;
        }
        public static SalaryComponent Create(
            Guid id,
            string name,
            string code,
            Guid categoryId,
            bool isTaxable,
            bool isPfapplicable,
            bool isEsiapplicable,
            bool isCtccomponent,
            bool isActive,
            int displayOrder,
            DateTime createdOn,
            Guid createdBy)
        {
            var salaryComponent = new SalaryComponent 
            { 
                Id = id,
                Name = name,
                Code = code,
                CategoryId = categoryId,
                IsTaxable = isTaxable,
                IsPfapplicable = isPfapplicable,
                IsEsiapplicable = isEsiapplicable,
                IsCtccomponent = isCtccomponent,
                IsActive = isActive,
                DisplayOrder = displayOrder,
                CreatedOn = createdOn,
                CreatedBy = createdBy
                };
            return salaryComponent;
        }
    }
}
