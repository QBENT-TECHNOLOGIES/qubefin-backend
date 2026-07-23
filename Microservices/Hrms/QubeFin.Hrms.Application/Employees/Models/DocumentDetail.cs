namespace QubeFin.Hrms.Application.Employees.Models;

public class DocumentDetailRequest
{
    public Guid Id { get;  set; }
    public string? DocumentCategory { get;  set; }
    public string DocumentName { get;  set; } = null!;
    public string? DocumentNo { get;  set; }
    public DateTime? ValidFrom { get;  set; }
    public DateTime? ValidTill { get;  set; }
    public string? FileName { get;  set; }
    public string? FileNo { get;  set; }
    public Guid EmployeeId { get;  set; }
}
