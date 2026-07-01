using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAdministrativeUnitType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public int LevelNo { get; set; }

    public string Category { get; set; } = null!;

    public virtual ICollection<TblAdministrativeUnit> TblAdministrativeUnits { get; set; } = new List<TblAdministrativeUnit>();
}
