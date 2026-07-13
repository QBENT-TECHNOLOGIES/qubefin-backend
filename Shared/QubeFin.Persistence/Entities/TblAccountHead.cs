using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAccountHead
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsSystemDefined { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblAccountGroup> TblAccountGroups { get; set; } = new List<TblAccountGroup>();
}
