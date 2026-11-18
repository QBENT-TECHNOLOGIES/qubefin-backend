namespace QubeFin.ImportEngine.Persistence.Models;

public class ImportJobRow
{
    public Guid Id { get; set; }
    public Guid ImportTypeId { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string StoredFilePath { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public string? ErrorDetails { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
