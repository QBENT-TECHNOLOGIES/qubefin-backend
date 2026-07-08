using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanProductParameter
{
    public Guid Id { get; set; }

    public Guid LoanProductId { get; set; }

    public string PaymentSchedule { get; set; } = null!;

    public decimal? MinLoanAmount { get; set; }

    public decimal? MaxLoanAmount { get; set; }

    public decimal? MinInterestRate { get; set; }

    public decimal? MaxInterestRate { get; set; }

    public decimal? MinLoginFees { get; set; }

    public decimal? MaxLoginFees { get; set; }

    public decimal? MaxCreditLimit { get; set; }

    public int? MoratoriumPeriod { get; set; }

    public string? InstallmentPeriod { get; set; }

    public string? Factor { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public Guid? AccountGroupId { get; set; }

    public Guid PrincipalGlaccount { get; set; }

    public Guid InterestGlaccount { get; set; }

    public Guid? InsuranceGlaccount { get; set; }

    public Guid? Sgstglaccount { get; set; }

    public Guid? Cgstglaccount { get; set; }

    public Guid? Igstglaccount { get; set; }

    public Guid? ProcessingFeesGlaccount { get; set; }

    public Guid? CbinspectionGlaccount { get; set; }

    public Guid? LoanWriteOffGlaccount { get; set; }

    public Guid? LoanRecoveryGlaccount { get; set; }

    public Guid? RoundoffGlaccount { get; set; }

    public Guid? CersaiGlaccount { get; set; }

    public Guid? DocumentGlaccount { get; set; }

    public Guid? BpiGlaccount { get; set; }

    public DateOnly EffectedFrom { get; set; }

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;
}
