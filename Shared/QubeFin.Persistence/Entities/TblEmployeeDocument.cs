using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeDocument
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

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;
}
