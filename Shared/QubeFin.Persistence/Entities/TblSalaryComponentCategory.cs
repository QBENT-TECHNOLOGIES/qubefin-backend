using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblSalaryComponentCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TblSalaryComponent> TblSalaryComponents { get; set; } = new List<TblSalaryComponent>();
}
