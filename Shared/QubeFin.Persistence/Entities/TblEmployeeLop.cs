using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeLop
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly AbsentDate { get; set; }

    public string? PayrollStatus { get; set; }
}
