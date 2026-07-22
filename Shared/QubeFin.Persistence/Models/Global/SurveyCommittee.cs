namespace QubeFin.Persistence.Models.Global
{
    public class SurveyCommittee
    {
        public Guid Id { get; private set; }
        public Guid EmployeeId { get; private set; }
        public bool IsLead { get; private set; }
        public bool IsActive { get; private set; }
        public DateOnly AssignedFrom { get; private set; }
        public DateOnly? AssignedTo { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        private SurveyCommittee() { }

        public SurveyCommittee
            (
                Guid id,
                Guid employeeId,
                bool isLead,
                bool isActive,
                DateOnly assignedFrom,
                DateOnly? assignedTo,
                Guid createdBy,
                DateTime createdOn,
                Guid? lastModifiedBy,
                DateTime? lastModifiedOn
            )
        {
            Id = id;
            EmployeeId = employeeId;
            IsLead = isLead;
            IsActive = isActive;
            AssignedFrom = assignedFrom;
            AssignedTo = assignedTo;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = lastModifiedOn;
        }
        public static SurveyCommittee Create(Guid id, Guid employeeId, bool isLead, DateOnly assignedFrom, Guid createdBy)
        {
            var salaryComponent = new SurveyCommittee
            {
                Id = id,
                EmployeeId = employeeId,
                IsLead = isLead,
                IsActive = true,
                AssignedFrom = assignedFrom,
                CreatedBy = createdBy,
                CreatedOn = DateTime.Now,
            };
            return salaryComponent;
        }
        public void Update(bool isLead, bool isActive, DateOnly? assignedTo, Guid? lastModifiedBy)
        {
            IsLead = isLead;
            IsActive = isActive;
            AssignedTo = assignedTo;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = DateTime.Now;
        }
    }
}
