namespace QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence.Models.Global
{
    public class OrganizationUnit
    {
    public Guid Id { get; set; }
    public Guid OrganizationUnitTypeId { get; set; }
    public string Name { get; set; } = null!;
    public int CodeVal { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public TimeOnly? AttendanceInTime { get; set; }
    public TimeOnly? AttendanceOutTime { get; set; }

        public Guid OrganizationUnitTypeId { get; private set; }
        public string? OrganizationUnitType { get; private set;  }

        public string Name { get; private set; } = null!;

        public int CodeVal { get; private set; }

        public Guid? ParentId { get; private set; }

        public DateTime CreatedOn { get; private set; }

        public Guid CreatedBy { get; private set; }
        public string? CreatedByName { get; private set; }

        public DateTime? LastModifiedOn { get; private set; }

        public Guid? LastModifiedBy { get; private set; }
        public string? LastModifiedByName { get; private set; }

        public TimeOnly? AttendanceInTime { get; private set; }

        public TimeOnly? AttendanceOutTime { get; private set; }
        private OrganizationUnit() { }

    public OrganizationUnit(Guid id, Guid organizationUnitTypeId, string name, int codeVal, Guid? parentId, Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn,
        TimeOnly? attendanceInTime, TimeOnly? attendanceOutTime)
        {
            Id = id;
            OrganizationUnitTypeId = organizationUnitTypeId;
            Name = name;
            CodeVal = codeVal;
            ParentId = parentId;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            LastModifiedOn = lastModifiedOn;
            LastModifiedBy = lastModifiedBy;
            AttendanceInTime = attendanceInTime;
            AttendanceOutTime = attendanceOutTime;
        }

    public static OrganizationUnit Create(Guid id, Guid organizationUnitTypeId, string name, int codeVal, Guid? parentId, Guid createdBy,
        TimeOnly? attendanceInTime, TimeOnly? attendanceOutTime)
        {
            var organizationUnit = new OrganizationUnit
            {
                Id = id,
                OrganizationUnitTypeId = organizationUnitTypeId,
                Name = name,
                CodeVal = codeVal,
                ParentId = parentId,
            AttendanceInTime = attendanceInTime,
            AttendanceOutTime = attendanceOutTime,
                CreatedBy = createdBy,
            CreatedOn = DateTime.Now
            };

            return organizationUnit;
        }
        public void Update(Guid organizationUnitTypeId,
            string name,
            int codeVal,
            Guid? parentId,
            Guid? lastModifiedBy,
            TimeOnly? attendanceInTime,
            TimeOnly? attendanceOutTime)
        {
            Name = name;
            OrganizationUnitTypeId = organizationUnitTypeId;
            CodeVal = codeVal;
            ParentId = parentId;
            LastModifiedBy = lastModifiedBy;
            LastModifiedOn = DateTime.Now;
            AttendanceInTime = attendanceInTime;
            AttendanceOutTime = attendanceOutTime;
        }
        public void SetTypeAndNames(string organizationUnitType, string createdByName, string? lastModifiedByName)
        {
            OrganizationUnitType = organizationUnitType;
            CreatedByName = createdByName;
            LastModifiedByName = lastModifiedByName;
        }
             
    }
}
