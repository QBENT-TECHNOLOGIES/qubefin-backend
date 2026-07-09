using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCompanyBankAccount
{
    public Guid Id { get; set; }

    public Guid FinancialInstituteId { get; set; }

    public string AccountNo { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string IfscCode { get; set; } = null!;

    public Guid LedgerId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblFinancialInstitute FinancialInstitute { get; set; } = null!;

    public virtual TblAccountLedger Ledger { get; set; } = null!;

    public virtual ICollection<TblLoanApplication> TblLoanApplications { get; set; } = new List<TblLoanApplication>();
}
