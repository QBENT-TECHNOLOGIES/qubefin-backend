using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanWorkflowStep
{
    public Guid Id { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid ApplicationStepId { get; set; }

    public bool IsEditable { get; set; }

    public virtual TblApplicationStep ApplicationStep { get; set; } = null!;

    public virtual TblLoanWorkflow Workflow { get; set; } = null!;
}
