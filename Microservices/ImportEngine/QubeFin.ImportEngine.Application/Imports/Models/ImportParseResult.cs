namespace QubeFin.ImportEngine.Application.Imports.Models;

public class ImportParseResult
{
    public int TotalRows { get; set; }
    public List<Dictionary<string, string?>> Rows { get; } = [];
    public List<ImportRowError> Errors { get; } = [];
    public List<string> MissingHeaders { get; } = [];

    public bool IsSuccess => MissingHeaders.Count == 0 && Errors.Count == 0;
}
