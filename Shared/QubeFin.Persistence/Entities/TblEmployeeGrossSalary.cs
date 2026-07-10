using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeGrossSalary
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTill { get; set; }

    public decimal GrossSalary { get; set; }
}
