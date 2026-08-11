using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApprovalWorkflowStep
{
    public Guid Id { get; set; }

    public Guid ApprovalWorkflowId { get; set; }

    public Guid ReceiverPostId { get; set; }

    public bool IsRecommendEvent { get; set; }

    public bool IsApprovalEvent { get; set; }

    public string EventStatus { get; set; } = null!;

    public string EventButtonText { get; set; } = null!;

    public int SequenceNo { get; set; }

    public virtual TblApprovalWorkflow ApprovalWorkflow { get; set; } = null!;

    public virtual TblPost ReceiverPost { get; set; } = null!;
}
