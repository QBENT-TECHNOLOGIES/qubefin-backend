using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveStatus
{
    public byte Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsFinal { get; set; }

    public virtual ICollection<TblLeaveRequest> TblLeaveRequests { get; set; } = new List<TblLeaveRequest>();
}
