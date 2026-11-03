using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcmemberTransfer
{
    public Guid Id { get; set; }

    public Guid BcmemberId { get; set; }

    public Guid BcgroupId { get; set; }

    public DateOnly EffectedFrom { get; set; }

    public DateOnly? EffectedTo { get; set; }

    public virtual TblBcgroup Bcgroup { get; set; } = null!;

    public virtual TblBcmember Bcmember { get; set; } = null!;
}
