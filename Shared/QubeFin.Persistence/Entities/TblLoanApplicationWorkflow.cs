using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanApplicationWorkflow
{
    public Guid Id { get; set; }

    public Guid LoanApplicationId { get; set; }

    public Guid WorkflowId { get; set; }

    public Guid DesignationId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? WorkflowComment { get; set; }

    public int Sequence { get; set; }

    public bool IsApproved { get; set; }

    public bool IsRechecked { get; set; }

    public bool IsAcknowladge { get; set; }

    public bool IsSubmit { get; set; }

    public string? Otp { get; set; }

    public bool IsVerifyOtp { get; set; }

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblLoanApplication LoanApplication { get; set; } = null!;

    public virtual ICollection<TblLoanApplicationWorkflowDocument> TblLoanApplicationWorkflowDocuments { get; set; } = new List<TblLoanApplicationWorkflowDocument>();

    public virtual ICollection<TblLoanApplicationWorkflowStep> TblLoanApplicationWorkflowSteps { get; set; } = new List<TblLoanApplicationWorkflowStep>();

    public virtual TblUser? User { get; set; }

    public virtual TblLoanWorkflow Workflow { get; set; } = null!;
}
