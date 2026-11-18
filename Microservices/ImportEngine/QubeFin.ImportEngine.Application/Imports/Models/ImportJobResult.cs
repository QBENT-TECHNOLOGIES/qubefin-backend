namespace QubeFin.ImportEngine.Application.Imports.Models;

public class ImportJobResult
{
    public Guid JobId { get; set; }
    public string Status { get; set; } = null!;
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public List<ImportRowError> Errors { get; set; } = [];
}

public class ImportTypeSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? TemplateUrl { get; set; }
}
