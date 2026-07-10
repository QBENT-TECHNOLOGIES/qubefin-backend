using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblOrganizationUnitType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public int LevelNo { get; set; }

    public virtual ICollection<TblOrganizationUnit> TblOrganizationUnits { get; set; } = new List<TblOrganizationUnit>();
}
