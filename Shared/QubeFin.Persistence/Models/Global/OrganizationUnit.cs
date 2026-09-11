namespace QubeFin.Persistence.Models.Global;

public class OrganizationUnit
{
    public Guid Id { get; set; }
    public Guid OrganizationUnitTypeId { get; set; }
    public string Name { get; set; } = null!;
    public int CodeVal { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? CompanyId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public TimeOnly? AttendanceInTime { get; set; }
    public TimeOnly? AttendanceOutTime { get; set; }
    public int? CheckRadiusInMeter { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }

    private OrganizationUnit() { }

    public OrganizationUnit(Guid id, Guid organizationUnitTypeId, string name, int codeVal, decimal? latitude, decimal? longitude, TimeOnly? attendanceInTime, TimeOnly? attendanceOutTime, 
        int? checkRadiusInMeter, Guid? parentId, Guid? companyId, Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
    {
        Id = id;
        OrganizationUnitTypeId = organizationUnitTypeId;
        Name = name;
        CodeVal = codeVal;
        ParentId = parentId;
        CompanyId= companyId;
        Latitude = latitude; 
        Longitude = longitude;
        AttendanceInTime = attendanceInTime;
        AttendanceOutTime = attendanceOutTime;
        CheckRadiusInMeter = checkRadiusInMeter;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }

    public static OrganizationUnit Create(Guid id, Guid organizationUnitTypeId, string name, int codeVal, Guid? parentId, Guid? companyId, decimal? latitude, decimal? longitude,
        TimeOnly? attendanceInTime, TimeOnly? attendanceOutTime, int? checkRadiusInMeter, Guid createdBy)
    {
        var organizationUnit = new OrganizationUnit
        {
            Id = id,
            OrganizationUnitTypeId = organizationUnitTypeId,
            Name = name,
            CodeVal = codeVal,
            ParentId = parentId,
            CompanyId = companyId,
            Latitude = latitude, 
            Longitude = longitude,
            AttendanceInTime = attendanceInTime,
            AttendanceOutTime = attendanceOutTime,
            CheckRadiusInMeter = checkRadiusInMeter,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now
        };

        return organizationUnit;
    }
    public void Update(Guid organizationUnitTypeId,
        string name,
        int codeVal,
        decimal? latitude, 
        decimal? longitude,
        TimeOnly? attendanceInTime,
        TimeOnly? attendanceOutTime,
        int? checkRadiusInMeter,
        Guid? parentId,
        Guid? companyId,
        Guid lastModifiedBy)
    {
        Name = name;
        OrganizationUnitTypeId = organizationUnitTypeId;
        CodeVal = codeVal;
        ParentId = parentId;
        CompanyId = companyId;
        Latitude = latitude;
        Longitude = longitude;
        AttendanceInTime = attendanceInTime;
        AttendanceOutTime = attendanceOutTime;
        CheckRadiusInMeter = checkRadiusInMeter;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = DateTime.Now;
    }
    //public void SetTypeAndNames(string organizationUnitType, string createdByName, string? lastModifiedByName)
    //{
    //    OrganizationUnitType = organizationUnitType;
    //    CreatedByName = createdByName;
    //    LastModifiedByName = lastModifiedByName;
    //}

}