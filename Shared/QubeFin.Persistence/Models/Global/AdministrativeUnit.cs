namespace QubeFin.Persistence.Models.Global;

public class AdministrativeUnit
{
    public Guid Id { get; set; }
    public Guid AdministrativeUnitTypeId { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public Guid? LastModifiedBy { get; set; }

    private AdministrativeUnit() { }

    public AdministrativeUnit(Guid id, Guid administrativrUnitTypeId, string name, Guid? parentId, bool isActive, Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn)
    {
        Id = id;
        AdministrativeUnitTypeId = administrativrUnitTypeId;
        Name = name;
        ParentId = parentId;
        IsActive = isActive;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
    }

    public static AdministrativeUnit Create(Guid id, Guid administrativrUnitTypeId, string name, Guid? parentId, Guid createdBy)
    {
        var administrativeUnit = new AdministrativeUnit
        {
            Id = id,
            AdministrativeUnitTypeId = administrativrUnitTypeId,
            Name = name,
            ParentId = parentId,
            // A newly created unit is always active; deactivating is an update-time action.
            IsActive = true,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now
        };

        return administrativeUnit;
    }

    public void Update(Guid administrativrUnitTypeId, string name, Guid? parentId, bool isActive, Guid userId)
    {
        AdministrativeUnitTypeId = administrativrUnitTypeId;
        Name = name;
        ParentId = parentId;
        IsActive = isActive;
        LastModifiedBy = userId;
        LastModifiedOn = DateTime.UtcNow;
    }
}
