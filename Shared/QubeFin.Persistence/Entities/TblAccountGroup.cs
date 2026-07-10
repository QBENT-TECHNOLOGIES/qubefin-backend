using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAccountGroup
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int CodeSequence { get; set; }

    public string Code { get; set; } = null!;

    public Guid AccHeadId { get; set; }

    public Guid? ParentId { get; set; }

    public bool IsSystemDefined { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblAccountHead AccHead { get; set; } = null!;

    public virtual ICollection<TblAccountGroup> InverseParent { get; set; } = new List<TblAccountGroup>();

    public virtual TblAccountGroup? Parent { get; set; }

    public virtual ICollection<TblLoanProduct> TblLoanProducts { get; set; } = new List<TblLoanProduct>();
}
