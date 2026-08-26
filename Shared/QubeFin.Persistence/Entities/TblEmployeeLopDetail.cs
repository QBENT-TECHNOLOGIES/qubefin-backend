using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeLopDetail
{
    public Guid Id { get; set; }

    public Guid EmployeeLopId { get; set; }

    public DateOnly AbsentDate { get; set; }

    public string? PayrollStatus { get; set; }

    public virtual TblEmployeeLop EmployeeLop { get; set; } = null!;
}
