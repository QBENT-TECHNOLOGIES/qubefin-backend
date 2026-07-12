using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLeaveType
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public bool IsPrayerable { get; set; }

    public bool IsConversionLeave { get; set; }

    public bool IsEncashable { get; set; }

    public int NoOfDaysEntitled { get; set; }

    public int? NoOfDaysCapped { get; set; }

    public int MaxContinuousDays { get; set; }

    public bool NegativeBalanceAllowed { get; set; }

    public byte[] ConcurrencyStamp { get; set; } = null!;

    public int? SeqNo { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblLeaveTransaction> TblLeaveTransactions { get; set; } = new List<TblLeaveTransaction>();
}
