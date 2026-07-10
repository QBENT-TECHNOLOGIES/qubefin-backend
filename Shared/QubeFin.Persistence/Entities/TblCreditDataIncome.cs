using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataIncome
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public DateTime? ReportDate { get; set; }

    public string? Occupation { get; set; }

    public string? MonthlyIncome { get; set; }

    public string? MonthlyExpense { get; set; }

    public string? AssetOwnership { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
