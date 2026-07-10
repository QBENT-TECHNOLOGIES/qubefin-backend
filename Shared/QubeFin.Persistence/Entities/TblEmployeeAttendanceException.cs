using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeAttendanceException
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public int LateCount { get; set; }

    public int RegularizeCount { get; set; }

    public int PenaltyLeaveCount { get; set; }

    public int NegativeElcount { get; set; }

    public Guid? PayrollId { get; set; }

    public DateOnly? ProcessDate { get; set; }

    public bool IsApproved { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedOn { get; set; }
}
