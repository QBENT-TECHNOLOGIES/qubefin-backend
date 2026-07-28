using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAttendanceRegularization
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly RegularizationDate { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime AppliedOn { get; set; }

    public bool IsSubmit { get; set; }

    public DateTime? SubmitOn { get; set; }

    public bool IsApproved { get; set; }

    public bool IsRejected { get; set; }

    public Guid? ActivityBy { get; set; }

    public DateTime? ActivityOn { get; set; }

    public string? Attachment { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;
}
