using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPermission
{
    public string PermissionToken { get; set; } = null!;

    public int DisplayPosition { get; set; }

    public virtual ICollection<TblMenuPermission> TblMenuPermissions { get; set; } = new List<TblMenuPermission>();
}
