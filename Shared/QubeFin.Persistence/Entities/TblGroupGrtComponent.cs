using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupGrtComponent
{
    public Guid Id { get; set; }

    public Guid GrtId { get; set; }

    public byte ComponentNo { get; set; }

    public bool? Verified { get; set; }

    public string? Remarks { get; set; }

    public virtual TblGroupGrt Grt { get; set; } = null!;
}
