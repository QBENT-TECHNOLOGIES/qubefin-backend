using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveStatus
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsFinal { get; set; }
}
