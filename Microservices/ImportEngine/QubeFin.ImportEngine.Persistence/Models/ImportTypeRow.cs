namespace QubeFin.ImportEngine.Persistence.Models;

public class ImportTypeRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SpName { get; set; } = null!;
    public string? TemplateUrl { get; set; }
    public string Columns { get; set; } = null!;
    public bool IsActive { get; set; }
}
