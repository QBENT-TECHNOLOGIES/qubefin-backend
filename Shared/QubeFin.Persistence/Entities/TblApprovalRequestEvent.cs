using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApprovalRequestEvent
{
    public Guid Id { get; set; }

    public string Category { get; set; } = null!;

    public Guid MappingId { get; set; }

    public Guid ApprovalWorflowEventId { get; set; }

    public Guid SenderDesignationId { get; set; }

    public Guid ReceiverDesignationId { get; set; }

    public Guid? NextApprovalWorflowEventId { get; set; }

    public DateTime EventDate { get; set; }

    public string? Remarks { get; set; }

    public virtual TblDesignation ReceiverDesignation { get; set; } = null!;

    public virtual TblDesignation SenderDesignation { get; set; } = null!;
}
