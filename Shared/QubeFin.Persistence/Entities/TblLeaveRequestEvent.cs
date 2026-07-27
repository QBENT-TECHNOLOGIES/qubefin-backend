using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveRequestEvent
{
    public Guid Id { get; set; }

    public Guid LeaveRequestId { get; set; }

    public Guid LeaveEventId { get; set; }

    public Guid SenderDesignationId { get; set; }

    public Guid ReceiverDesignationId { get; set; }

    public Guid? NextLeaveEventId { get; set; }

    public DateTime EventDate { get; set; }

    public string? Remarks { get; set; }

    public virtual TblLeaveEvent LeaveEvent { get; set; } = null!;

    public virtual TblLeaveRequest LeaveRequest { get; set; } = null!;

    public virtual TblLeaveEvent? NextLeaveEvent { get; set; }

    public virtual TblDesignation ReceiverDesignation { get; set; } = null!;

    public virtual TblDesignation SenderDesignation { get; set; } = null!;
}
