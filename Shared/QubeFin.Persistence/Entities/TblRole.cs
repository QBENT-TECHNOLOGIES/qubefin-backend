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

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblUser? LastModifiedByNavigation { get; set; }

    public virtual ICollection<TblDesignationRole> TblDesignationRoles { get; set; } = new List<TblDesignationRole>();

    public virtual ICollection<TblRoleMenuPermission> TblRoleMenuPermissions { get; set; } = new List<TblRoleMenuPermission>();
}
