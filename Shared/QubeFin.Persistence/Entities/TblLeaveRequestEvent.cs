using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveRequestEvent
{
    public Guid Id { get; set; }

    public Guid LeaveRequestId { get; set; }

    public Guid LeaveEventId { get; set; }

    public Guid CurrentEmployeeId { get; set; }

    public Guid? NextEmployeeId { get; set; }

    public int EventSeqNo { get; set; }

    public DateTime EventDate { get; set; }

    public string? Remarks { get; set; }
}
