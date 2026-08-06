using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAttendanceRegularization
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string RegularizationType { get; set; } = null!;

    public string RegularizationDates { get; set; } = null!;

    public string? Reason { get; set; }

    public string? Attachment { get; set; }

    public string? Remarks { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;
}
