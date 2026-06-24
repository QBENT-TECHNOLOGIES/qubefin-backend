using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMenu
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public string? Target { get; set; }

    public Guid MenuGroupId { get; set; }

    public int DisplayPosition { get; set; }

    public bool IsActive { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblMenuGroup MenuGroup { get; set; } = null!;

    public virtual ICollection<TblRoleMenu> TblRoleMenus { get; set; } = new List<TblRoleMenu>();

    public virtual ICollection<TblUserMenu> TblUserMenus { get; set; } = new List<TblUserMenu>();
}
