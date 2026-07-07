namespace QubeFin.Persistence.Models.App;

public class MenuHierarchyItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Target { get; set; }
    public int Level { get; set; }
}
