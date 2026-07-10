using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataDependent
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public string? Relation { get; set; }

    public string? Name { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
