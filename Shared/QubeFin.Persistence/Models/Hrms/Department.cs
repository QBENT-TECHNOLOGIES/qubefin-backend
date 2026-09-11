namespace QubeFin.Persistence.Models.Hrms;

    public class Department
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;
    public Guid? HodEmployeeId { get; set; }

        public bool IsActive { get; private set; }

        public DateTime? CreatedOn { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public DateTime? LastModifiedOn { get; private set; }

        public Guid? LastModifiedBy { get; private set; }

    private Department() { }

    public Department(Guid id, string name, Guid hodEmployeeId, bool isActive, DateTime? createdOn, Guid? createdBy, DateTime? lastModifiedOn, Guid? lastModifiedBy)
    {
        Id = id;
        Name = name;
        HodEmployeeId = hodEmployeeId;
        IsActive = isActive;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }

    public static Department Create(Guid id, string name, Guid hodEmployeeId, bool isActive, Guid createdBy)
    {
        return new Department(id, name, hodEmployeeId, isActive, DateTime.Now, createdBy, null, null);
    }
    public void Update(string name, Guid hodEmployeeId, bool isActive, Guid modifiedBy)
    {
        Name = name;
        HodEmployeeId = hodEmployeeId;
        IsActive = isActive;
        LastModifiedOn = DateTime.Now;
        LastModifiedBy = modifiedBy;
    }
}

