using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUserMenu
{
    public Guid UserId { get; set; }

    public Guid MenuId { get; set; }

    public bool HasReadAccess { get; set; }

    public bool HasWriteAccess { get; set; }

    public bool HasFullAccess { get; set; }

    public virtual TblMenu Menu { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
