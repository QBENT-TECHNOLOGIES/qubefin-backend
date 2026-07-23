using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMenu
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public string? Target { get; set; }

    public Guid? ParentId { get; set; }

    public int DisplayPosition { get; set; }

    public bool IsActive { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<TblMenu> InverseParent { get; set; } = new List<TblMenu>();

    public virtual TblUser? LastModifiedByNavigation { get; set; }

    public virtual TblMenu? Parent { get; set; }

    public virtual ICollection<TblMenuPermission> TblMenuPermissions { get; set; } = new List<TblMenuPermission>();
}
