using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPayRollComponent
{
    public Guid Id { get; set; }

    public Guid PayRollId { get; set; }

    public Guid SalaryComponentId { get; set; }

    public decimal Percentage { get; set; }

    public decimal Amount { get; set; }

    public virtual TblPayRoll PayRoll { get; set; } = null!;

    public virtual TblSalaryComponent SalaryComponent { get; set; } = null!;
}
