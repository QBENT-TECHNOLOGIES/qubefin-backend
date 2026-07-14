using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveRequest
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid LeaveTypeId { get; set; }

    public Guid? FinYearId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public DateTime RequestDate { get; set; }

    public string? Reason { get; set; }

    public string? Address { get; set; }

    public int? TotalDays { get; set; }

    public byte Status { get; set; }

    public Guid CalenderYearId { get; set; }

    public virtual TblCalenderYear CalenderYear { get; set; } = null!;

    public virtual TblLeaveStatus StatusNavigation { get; set; } = null!;
}
