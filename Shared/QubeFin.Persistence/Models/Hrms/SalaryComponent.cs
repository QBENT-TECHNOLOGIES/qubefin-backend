namespace QubeFin.Persistence.Models.Hrms
{
    public class SalaryComponent
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public string Code { get; private set; } = null!;

        public Guid CategoryId { get; private set; }

        public bool IsTaxable { get; private set; }

        public bool IsPfapplicable { get; private set; }

        public bool IsEsiapplicable { get; private set; }

        public bool IsCtccomponent { get; private set; }

        public bool IsActive { get; private set; }

        public int DisplayOrder { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public Guid CreatedBy { get; private set; }

        public DateTime? LastModifiedOn { get; private set; }

        public Guid? LastModifiedBy { get; private set; }
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
                CreatedOn = DateTime.Now,
                CreatedBy = createdBy
            };
            return salaryComponent;
        }
        public void Update(
            string name,
            string code,
            Guid categoryId,
            bool isTaxable,
            bool isPfapplicable,
            bool isEsiapplicable,
            bool isCtccomponent,
            bool isActive,
            int displayOrder,
            Guid? lastModifiedBy)
        {
            Name = name;
            Code = code;
            CategoryId = categoryId;
            IsTaxable = isTaxable;
            IsPfapplicable = isPfapplicable;
            IsEsiapplicable = isEsiapplicable;
            IsCtccomponent = isCtccomponent;
            IsActive = isActive;
            DisplayOrder = displayOrder;
            LastModifiedOn = DateTime.Now;
            LastModifiedBy = lastModifiedBy;
        }
    }
}
