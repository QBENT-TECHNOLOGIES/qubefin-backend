using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblFinancialYear
{
    public Guid Id { get; set; }

    public string Caption { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsLocked { get; set; }

    public string? AssessmentYear { get; set; }

    public virtual ICollection<TblLeaveTransaction> TblLeaveTransactions { get; set; } = new List<TblLeaveTransaction>();

    public virtual ICollection<TblPayRoll> TblPayRolls { get; set; } = new List<TblPayRoll>();
}
