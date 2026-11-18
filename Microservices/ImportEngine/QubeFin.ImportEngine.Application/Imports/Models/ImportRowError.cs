namespace QubeFin.ImportEngine.Application.Imports.Models;

public class ImportRowError
{
    public int Row { get; set; }
    public string Column { get; set; } = null!;
    public string? Value { get; set; }
    public string Message { get; set; } = null!;
}
