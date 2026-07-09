using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblFinancialInstitute
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsBank { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual ICollection<TblCompanyBankAccount> TblCompanyBankAccounts { get; set; } = new List<TblCompanyBankAccount>();

    public virtual ICollection<TblEmployee> TblEmployees { get; set; } = new List<TblEmployee>();
}
