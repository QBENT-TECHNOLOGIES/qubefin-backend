using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPermission
{
    public Guid Id { get; set; }

    public string AccessFunction { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<TblRolePermission> TblRolePermissions { get; set; } = new List<TblRolePermission>();
}
