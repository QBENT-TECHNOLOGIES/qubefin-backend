using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveType
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public bool IsPrayerable { get; set; }

    public bool IsConvertible { get; set; }

    public bool IsEncashable { get; set; }

    public bool IsCarryForwarded { get; set; }

    public int NoOfDaysEntitled { get; set; }

    public int? NoOfDaysCapped { get; set; }

    public int MaxContinuousDays { get; set; }

    public bool ApplicableAfterProbation { get; set; }

    public bool IsMonthlyCredit { get; set; }

    public int SeqNo { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblUser? LastModifiedByNavigation { get; set; }

    public virtual ICollection<TblApprovalWorkflowEvent> TblApprovalWorkflowEvents { get; set; } = new List<TblApprovalWorkflowEvent>();

    public virtual ICollection<TblApprovalWorkflow> TblApprovalWorkflows { get; set; } = new List<TblApprovalWorkflow>();

    public virtual ICollection<TblLeavePrayer> TblLeavePrayers { get; set; } = new List<TblLeavePrayer>();

    public virtual ICollection<TblLeaveTransaction> TblLeaveTransactions { get; set; } = new List<TblLeaveTransaction>();
}
