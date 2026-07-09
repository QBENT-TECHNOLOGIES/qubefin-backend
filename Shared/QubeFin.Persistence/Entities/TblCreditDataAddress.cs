using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataAddress
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? Type { get; set; }

    public string? Address { get; set; }

    public string? State { get; set; }

    public int? Postal { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
