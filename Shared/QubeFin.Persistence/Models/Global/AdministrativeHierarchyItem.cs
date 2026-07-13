namespace QubeFin.Persistence.Models.Global;

public class AdministrativeHierarchyItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
    public int Level { get; set; }
}
