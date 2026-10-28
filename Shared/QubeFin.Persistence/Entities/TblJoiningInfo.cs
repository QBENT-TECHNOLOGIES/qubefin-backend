using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblJoiningInfo
{
    public Guid Id { get; set; }

    public string ReferenceNo { get; set; } = null!;

    public decimal MonthlyCtc { get; set; }

    public DateOnly OfferLetterDate { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblUser? LastModifiedByNavigation { get; set; }
}
