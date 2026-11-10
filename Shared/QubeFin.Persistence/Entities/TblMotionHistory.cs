using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMotionHistory
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string Motion { get; set; } = null!;

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string? Source { get; set; }
}
