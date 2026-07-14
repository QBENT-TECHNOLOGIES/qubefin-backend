using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveTransaction
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid LeaveTypeId { get; set; }

    public Guid FinYearId { get; set; }

    public decimal LeaveDebit { get; set; }

    public decimal LeaveCredit { get; set; }

    public DateOnly TransactionDate { get; set; }

    public Guid? LeaveRequestId { get; set; }

    public Guid? LeavePrayerId { get; set; }

    public Guid? LeaveEncashmentId { get; set; }

    public bool IsSystemCredit { get; set; }

    public string? Remarks { get; set; }

    public Guid CalenderYearId { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblFinancialYear FinYear { get; set; } = null!;

    public virtual TblLeaveType LeaveType { get; set; } = null!;
}
