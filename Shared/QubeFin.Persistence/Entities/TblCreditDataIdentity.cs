using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataIdentity
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? IdentityType { get; set; }

    public string? IdentityNo { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
