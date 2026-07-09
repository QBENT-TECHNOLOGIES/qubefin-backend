using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataAlert
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public string? AlertMsg { get; set; }

    public int Sequence { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
