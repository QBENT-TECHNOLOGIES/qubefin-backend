using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAccountLedger
{
    public Guid Id { get; set; }

    public int CodeSequence { get; set; }

    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Alias { get; set; } = null!;

    public Guid AccGroupId { get; set; }

    public decimal? OpeningBalance { get; set; }

    public DateOnly? OpeningDate { get; set; }

    public bool IsSystemDefined { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblCompanyBankAccount> TblCompanyBankAccounts { get; set; } = new List<TblCompanyBankAccount>();
}
