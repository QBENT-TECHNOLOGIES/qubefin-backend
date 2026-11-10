using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblJobHistory
{
    public Guid Id { get; set; }

    public Guid JobId { get; set; }

    public string Actions { get; set; } = null!;

    public Guid? PerformedById { get; set; }

    public DateTimeOffset PerformedAt { get; set; }

    public Guid? FromAssigneeId { get; set; }

    public Guid? ToAssigneeId { get; set; }

    public Guid? RelatedJobId { get; set; }

    public DateOnly? OldCompleteBy { get; set; }

    public DateOnly? NewCompleteBy { get; set; }

    public string? Remarks { get; set; }

    public virtual TblEmployee? FromAssignee { get; set; }

    public virtual TblJob Job { get; set; } = null!;

    public virtual TblUser? PerformedBy { get; set; }

    public virtual TblJob? RelatedJob { get; set; }

    public virtual TblEmployee? ToAssignee { get; set; }

    public virtual ICollection<TblJobTask> Tasks { get; set; } = new List<TblJobTask>();
}
