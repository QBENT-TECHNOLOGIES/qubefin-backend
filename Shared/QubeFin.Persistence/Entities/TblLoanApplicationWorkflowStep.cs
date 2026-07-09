using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanApplicationWorkflowStep
{
    public Guid Id { get; set; }

    public Guid LoanApplicationWorkflowId { get; set; }

    public Guid ApplicationStepId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedOn { get; set; }

    public bool IsSubmit { get; set; }

    public virtual TblApplicationStep ApplicationStep { get; set; } = null!;

    public virtual TblLoanApplicationWorkflow LoanApplicationWorkflow { get; set; } = null!;
}
