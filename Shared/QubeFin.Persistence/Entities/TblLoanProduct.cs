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

    public bool IsMicroLoan { get; set; }

    public bool IsPropertyLoan { get; set; }

    public bool HavingLegalStep { get; set; }

    public bool BpiChargeApplied { get; set; }

    public bool LoginFeesApplied { get; set; }

    public bool EnachRequired { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblLoanProductParameter> TblLoanProductParameters { get; set; } = new List<TblLoanProductParameter>();

    public virtual ICollection<TblLoanProductQuestion> TblLoanProductQuestions { get; set; } = new List<TblLoanProductQuestion>();

    public virtual ICollection<TblLoanProductStep> TblLoanProductSteps { get; set; } = new List<TblLoanProductStep>();

    public virtual ICollection<TblLoanSettingParameter> TblLoanSettingParameters { get; set; } = new List<TblLoanSettingParameter>();

    public virtual ICollection<TblLoanWorkflow> TblLoanWorkflows { get; set; } = new List<TblLoanWorkflow>();
}
