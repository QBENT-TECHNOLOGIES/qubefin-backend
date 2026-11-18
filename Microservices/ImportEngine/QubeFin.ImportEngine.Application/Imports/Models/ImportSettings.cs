namespace QubeFin.ImportEngine.Application.Imports.Models;

public class ImportSettings
{
    public int MaxRows { get; set; } = 50000;
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
}
