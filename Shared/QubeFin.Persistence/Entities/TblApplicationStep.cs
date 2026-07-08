using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblApplicationStep
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    public string? Color { get; set; }

    public bool ApprovalStep { get; set; }

    public bool IsAccountVerificationStep { get; set; }

    public bool IsDisburseStep { get; set; }

    public string Component { get; set; } = null!;

    public int Sequence { get; set; }

    public virtual ICollection<TblLoanProductStep> TblLoanProductSteps { get; set; } = new List<TblLoanProductStep>();

    public virtual ICollection<TblLoanWorkflowStep> TblLoanWorkflowSteps { get; set; } = new List<TblLoanWorkflowStep>();
}
