
using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeDocument
{
    public Guid Id { get; set; }
    public string DocumentCategory { get; set; } = null!;
    public string DocumentName { get; set; } = null!;
    public string? DocumentNo { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTill { get; set; }
    public string? FileName { get; set; }
    public string? FileNo { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime? UploadedOn { get; set; }
    public Guid? UploadedBy { get; set; }


    private EmployeeDocument() { }
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
        Guid? uploadedBy,
        DateTime? uploadedOn
    )
    {
        Id = id;
        DocumentCategory = documentCategory;
        DocumentName = documentName;
        DocumentNo = documentNo;
        ValidFrom = validFrom;
        ValidTill = validTill;
        FileName = fileName;
        FileNo = fileNo;
        EmployeeId = employeeId;
        UploadedBy = uploadedBy;
        UploadedOn = uploadedOn;
    }
    public static EmployeeDocument Create(
        //Guid employeeId,
        string documentCategory,
        string documentName,
        string? documentNo,
        DateOnly? validFrom,
        DateOnly? validTill,
        string? fileName,
        string? fileNo,
        Guid? uploadedBy)
    {
        var docs = new EmployeeDocument
        {
            Id = Guid.NewGuid(),
            //EmployeeId = employeeId,
            DocumentCategory = documentCategory,
            DocumentName = documentName,
            DocumentNo = documentNo,
            ValidFrom = validFrom,
            ValidTill = validTill,
            FileName = fileName,
            FileNo = fileNo,
            UploadedBy = uploadedBy,
            UploadedOn = DateTime.Now
        };

        return docs;
    }
}



