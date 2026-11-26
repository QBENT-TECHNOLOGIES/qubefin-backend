using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupCgtTopic
{
    public Guid Id { get; set; }

    public Guid CgtId { get; set; }

    public byte TopicNo { get; set; }

    public bool? Covered { get; set; }

    public string? Remarks { get; set; }

    public virtual TblGroupCgt Cgt { get; set; } = null!;
}
