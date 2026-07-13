using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanProduct
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public string? ProductCode { get; set; }

    public bool IsActive { get; set; }

    public bool IsPropertyLoan { get; set; }

    public bool HavingLegalStep { get; set; }

    public bool BpiChargeApplied { get; set; }

    public bool LoginFeesApplied { get; set; }

    public bool EnachRequired { get; set; }

    public string PaymentSchedule { get; set; } = null!;

    public Guid? AccountGroupId { get; set; }

    public Guid? PrincipalGlaccount { get; set; }

    public Guid? InterestGlaccount { get; set; }

    public Guid? InsuranceGlaccount { get; set; }

    public Guid? Sgstglaccount { get; set; }

    public Guid? Cgstglaccount { get; set; }

    public Guid? Igstglaccount { get; set; }

    public Guid? ProcessingFeesGlaccount { get; set; }

    public Guid? CbinspectionGlaccount { get; set; }

    public Guid? LoanWriteOffGlaccount { get; set; }

    public Guid? LoanRecoveryGlaccount { get; set; }

    public Guid? RoundoffGlaccount { get; set; }

    public Guid? CersaiGlaccount { get; set; }

    public Guid? DocumentGlaccount { get; set; }

    public Guid? BpiGlaccount { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblAccountGroup? AccountGroup { get; set; }

    public virtual ICollection<TblLoanApplication> TblLoanApplications { get; set; } = new List<TblLoanApplication>();

    public virtual ICollection<TblLoanProductQuestion> TblLoanProductQuestions { get; set; } = new List<TblLoanProductQuestion>();

    public virtual ICollection<TblLoanProductStep> TblLoanProductSteps { get; set; } = new List<TblLoanProductStep>();

    public virtual ICollection<TblLoanSettingParameter> TblLoanSettingParameters { get; set; } = new List<TblLoanSettingParameter>();

    public virtual ICollection<TblLoanWorkflow> TblLoanWorkflows { get; set; } = new List<TblLoanWorkflow>();
}
