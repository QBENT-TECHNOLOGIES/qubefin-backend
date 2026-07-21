namespace QubeFin.Global.Api.Requests;

public class AdministrativeUnitRequest
{
    public Guid AdministrativeUnitTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ParentId { get; set; }
}
