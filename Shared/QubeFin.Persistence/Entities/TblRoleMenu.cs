using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblRoleMenu
{
    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public bool HasReadAccess { get; set; }

    public bool HasWriteAccess { get; set; }

    public bool HasFullAccess { get; set; }

    public virtual TblMenu Menu { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
