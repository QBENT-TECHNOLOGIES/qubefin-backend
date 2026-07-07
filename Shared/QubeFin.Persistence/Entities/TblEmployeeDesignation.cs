using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeDesignation
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid DesignationId { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;
}
