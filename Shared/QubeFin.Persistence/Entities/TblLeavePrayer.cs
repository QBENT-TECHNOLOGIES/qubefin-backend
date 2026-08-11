using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeavePrayer
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid LeaveTypeId { get; set; }

    public int PrayerDays { get; set; }

    public string? Attachment { get; set; }

    public string? Remarks { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblLeaveType LeaveType { get; set; } = null!;
}
