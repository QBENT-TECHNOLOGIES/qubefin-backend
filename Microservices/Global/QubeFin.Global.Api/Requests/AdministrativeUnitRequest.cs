namespace QubeFin.Global.Api.Requests;

public class AdministrativeUnitRequest
{
    public Guid AdministrativeUnitTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Only honoured on update: a newly created unit is always active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// A root level unit has no parent. Clients that omit the parent send an empty guid, which
    /// would otherwise be stored as a parent that does not exist and drop the unit off the tree.
    /// </summary>
    public Guid? NormalizedParentId => ParentId == Guid.Empty ? null : ParentId;
}
