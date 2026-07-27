using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUserMenuPermission
{
    public Guid UserId { get; set; }

    public Guid MenuPermissionId { get; set; }

    public string AccessClaimToken { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual TblMenuPermission MenuPermission { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
