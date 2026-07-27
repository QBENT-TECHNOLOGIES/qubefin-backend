using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblRolePermission
{
    public Guid RoleMenuId { get; set; }

    public string PermissionToken { get; set; } = null!;

    public string AccessClaimToken { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public virtual TblPermission PermissionTokenNavigation { get; set; } = null!;

    public virtual TblRoleMenu RoleMenu { get; set; } = null!;
}
