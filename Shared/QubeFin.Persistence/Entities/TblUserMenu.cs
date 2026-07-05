using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUserMenu
{
    public Guid UserId { get; set; }

    public Guid MenuId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual TblMenu Menu { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
