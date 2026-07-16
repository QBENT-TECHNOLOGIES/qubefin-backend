using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCalenderYear
{
    public Guid Id { get; set; }

    public string Caption { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsLocked { get; set; }

    public virtual ICollection<TblLeaveRequest> TblLeaveRequests { get; set; } = new List<TblLeaveRequest>();
}
