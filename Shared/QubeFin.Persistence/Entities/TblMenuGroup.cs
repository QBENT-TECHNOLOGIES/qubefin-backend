using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMenuGroup
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int DisplayPosition { get; set; }

    public int? Sequence { get; set; }

    public string? Category { get; set; }

    public string? Target { get; set; }

    public string? Icon { get; set; }

    public virtual ICollection<TblMenu> TblMenus { get; set; } = new List<TblMenu>();
}
