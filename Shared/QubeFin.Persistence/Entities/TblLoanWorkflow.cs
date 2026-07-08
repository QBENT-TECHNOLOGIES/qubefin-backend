using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanWorkflow
{
    public Guid Id { get; set; }

    public Guid LoanProductId { get; set; }

    public Guid PostId { get; set; }

    public bool IsReverse { get; set; }

    public bool IsApprovalAccess { get; set; }

    public bool IsCheckForCluster { get; set; }

    public bool IsAutofill { get; set; }

    public string? WorkflowStatus { get; set; }

    public int Sequence { get; set; }

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;

    public virtual TblPost Post { get; set; } = null!;

    public virtual ICollection<TblLoanWorkflowDocument> TblLoanWorkflowDocuments { get; set; } = new List<TblLoanWorkflowDocument>();

    public virtual ICollection<TblLoanWorkflowStep> TblLoanWorkflowSteps { get; set; } = new List<TblLoanWorkflowStep>();
}
