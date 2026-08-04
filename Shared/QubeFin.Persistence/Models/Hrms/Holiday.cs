namespace QubeFin.Persistence.Models.Hrms;

public class Holiday
{
    public Guid Id { get; private set; }
    public Guid OrgUnitId { get; private set; }
    public DateOnly HolidayDate { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    private Holiday()
    {
    }

    public Holiday(
        Guid id,
        Guid orgUnitId,
        DateOnly holidayDate,
        string description,
        DateTime createdOn,
        Guid createdBy,
        DateTime? lastModifiedOn,
        Guid? lastModifiedBy)
    {
        Id = id;
        OrgUnitId = orgUnitId;
        HolidayDate = holidayDate;
        Description = description;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }

    public static Holiday Create(Guid id, Guid orgUnitId, DateOnly holidayDate, string description, Guid createdBy)
    {
        return new Holiday(id, orgUnitId, holidayDate, description, DateTime.Now, createdBy, null, null);
    }

    public void Update(Guid orgUnitId, DateOnly holidayDate, string description, Guid modifiedBy)
    {
        OrgUnitId = orgUnitId;
        HolidayDate = holidayDate;
        Description = description;
        LastModifiedOn = DateTime.Now;
        LastModifiedBy = modifiedBy;
    }
}
