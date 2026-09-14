using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TmpLeaveBalanceLop
{
    public string EmpCode { get; set; } = null!;

    public Guid? Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Clbalance { get; set; }

    public decimal? Elbalance { get; set; }

    public decimal? Mlbalance { get; set; }

    public int? Lop { get; set; }

    public string? RoleName { get; set; }
}
