using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPermission
{
    public Guid Id { get; set; }

    public string PermissionToken { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public string BackgroundClass { get; set; } = null!;

    public string IconClass { get; set; } = null!;

    public int DisplayPosition { get; set; }

    public virtual ICollection<TblMenuPermission> TblMenuPermissions { get; set; } = new List<TblMenuPermission>();
}
