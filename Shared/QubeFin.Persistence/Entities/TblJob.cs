using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblJob
{
    public Guid Id { get; set; }

    public string JobCode { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string? JobDescription { get; set; }

    public Guid AssignTo { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsCompleted { get; set; }

    public DateOnly? CompletedDate { get; set; }

    public Guid? CompletedBy { get; set; }

    public Guid? TransferJobRefId { get; set; }

    public bool IsCancel { get; set; }

    public DateOnly? CancelDate { get; set; }

    public Guid? CanceledBy { get; set; }

    public string? CancelReason { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblDesignation AssignToNavigation { get; set; } = null!;

    public virtual TblUser? CanceledByNavigation { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<TblJob> InverseTransferJobRef { get; set; } = new List<TblJob>();

    public virtual TblUser? ModifiedByNavigation { get; set; }

    public virtual ICollection<TblJobHistory> TblJobHistoryJobs { get; set; } = new List<TblJobHistory>();

    public virtual ICollection<TblJobHistory> TblJobHistoryRelatedJobs { get; set; } = new List<TblJobHistory>();

    public virtual ICollection<TblJobTask> TblJobTasks { get; set; } = new List<TblJobTask>();

    public virtual TblJob? TransferJobRef { get; set; }
}
