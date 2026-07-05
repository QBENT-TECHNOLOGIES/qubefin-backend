using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblRoleMenu
{
    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual TblMenu Menu { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
