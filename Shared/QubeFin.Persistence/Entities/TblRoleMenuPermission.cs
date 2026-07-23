using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblRoleMenuPermission
{
    public Guid RoleId { get; set; }

    public Guid MenuPermissionId { get; set; }

    public string AccessClaimToken { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual TblMenuPermission MenuPermission { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
