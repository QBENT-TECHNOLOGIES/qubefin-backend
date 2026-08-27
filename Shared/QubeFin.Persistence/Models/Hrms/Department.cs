namespace QubeFin.Persistence.Models.Hrms;

    public class Department
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public DateTime? CreatedOn { get; private set; }

        public Guid? CreatedBy { get; private set; }

        public DateTime? LastModifiedOn { get; private set; }

        public Guid? LastModifiedBy { get; private set; }

    private Department() { }

    public Department(Guid id, string name, bool isActive, DateTime? createdOn, Guid? createdBy, DateTime? lastModifiedOn, Guid? lastModifiedBy)
    {
        Id = id;
        Name = name;
        IsActive = isActive;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }

    public static Department Create(Guid id, string name, bool isActive, Guid createdBy)
    {
        return new Department(id, name, isActive, DateTime.Now, createdBy, null, null);
    }
    public void Update(string name, bool isActive, Guid modifiedBy)
    {
        Name = name;
        IsActive = isActive;
        LastModifiedOn = DateTime.Now;
        LastModifiedBy = modifiedBy;
    }
}

