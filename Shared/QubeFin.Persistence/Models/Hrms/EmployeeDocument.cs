
using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeDocument
{
    public Guid Id { get; private set; }
    public string DocumentCategory { get; private set; } = null!;
    public string DocumentName { get; private set; } = null!;
    public string? DocumentNo { get; private set; }
    public DateOnly? ValidFrom { get; private set; }
    public DateOnly? ValidTill { get; private set; }
    public string? FileName { get; private set; }
    public string? FileNo { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime? UploadedOn { get; private set; }
    public Guid? UploadedBy { get; private set; }
    public DateTime? CreatedOn { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    // Required for ORM/EF Core hydration
    private EmployeeDocument()
    {
    }

    public EmployeeDocument(
        Guid id,
        string documentCategory,
        string documentName,
        string? documentNo,
        DateOnly? validFrom,
        DateOnly? validTill,
        string? fileName,
        string? fileNo,
        Guid employeeId,
        Guid? uploadedBy)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        DocumentCategory = documentCategory.Trim();
        DocumentName = documentName.Trim();
        DocumentNo = documentNo?.Trim();
        ValidFrom = validFrom;
        ValidTill = validTill;
        FileName = fileName?.Trim();
        FileNo = fileNo?.Trim();
        EmployeeId = employeeId;
        UploadedOn = DateTime.Now;
        UploadedBy = uploadedBy;
    }
}





