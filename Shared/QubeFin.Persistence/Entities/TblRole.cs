using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblRole
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<TblDesignationRole> TblDesignationRoles { get; set; } = new List<TblDesignationRole>();

    public virtual ICollection<TblRoleMenu> TblRoleMenus { get; set; } = new List<TblRoleMenu>();

    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();
}
