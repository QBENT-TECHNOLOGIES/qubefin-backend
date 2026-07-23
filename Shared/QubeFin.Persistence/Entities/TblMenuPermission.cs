using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMenuPermission
{
    public Guid Id { get; set; }

    public Guid MenuId { get; set; }

    public string PermissionToken { get; set; } = null!;

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblMenu Menu { get; set; } = null!;

    public virtual TblPermission PermissionTokenNavigation { get; set; } = null!;

    public virtual ICollection<TblRoleMenuPermission> TblRoleMenuPermissions { get; set; } = new List<TblRoleMenuPermission>();

    public virtual ICollection<TblUserMenuPermission> TblUserMenuPermissions { get; set; } = new List<TblUserMenuPermission>();
}
