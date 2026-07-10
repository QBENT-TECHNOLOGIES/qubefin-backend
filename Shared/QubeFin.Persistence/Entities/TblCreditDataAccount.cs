using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCreditDataAccount
{
    public Guid Id { get; set; }

    public Guid CreditDataId { get; set; }

    public string? Institution { get; set; }

    public string? AccountNumber { get; set; }

    public string? LoanCategory { get; set; }

    public string? LoanPurpose { get; set; }

    public string? Status { get; set; }

    public string? LoanCycleId { get; set; }

    public string? RepaymentFreq { get; set; }

    public decimal? SanctionAmount { get; set; }

    public decimal? DisburseAmount { get; set; }

    public decimal? CurrentBalance { get; set; }

    public decimal? PastDueAmount { get; set; }

    public decimal? LastPayment { get; set; }

    public decimal? WriteoffAmount { get; set; }

    public DateTime? SanctionDate { get; set; }

    public DateTime? ReportDate { get; set; }

    public DateTime? OpenDate { get; set; }

    public DateTime? CloseDate { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    public DateTime? WriteoffDate { get; set; }

    public int? NoofInstallment { get; set; }

    public decimal? InstallAmount { get; set; }

    public string? KeyPersonName { get; set; }

    public string? KeyPersonRel { get; set; }

    public string? NomineeName { get; set; }

    public string? NomineeRel { get; set; }

    public string? WriteoffReason { get; set; }

    public int? TotalAcc { get; set; }

    public int? NoOfActiveAcc { get; set; }

    public int? NoOfClosedAcc { get; set; }

    public int? NoOfPastDueAcc { get; set; }

    public int? NoOfWrittenOffAcc { get; set; }

    public virtual TblCreditDatum CreditData { get; set; } = null!;
}
