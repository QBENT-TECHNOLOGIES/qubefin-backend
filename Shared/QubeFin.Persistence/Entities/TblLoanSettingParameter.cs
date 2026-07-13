using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblLoanSettingParameter
{
    public Guid Id { get; set; }

    public Guid LoanProductId { get; set; }

    public decimal? MinLoanAmount { get; set; }

    public decimal? MaxLoanAmount { get; set; }

    public decimal? MinInterestRate { get; set; }

    public decimal? MaxInterestRate { get; set; }

    public decimal? MinLoginFees { get; set; }

    public decimal? MaxLoginFees { get; set; }

    public string? InstallmentPeriod { get; set; }

    public string? Factor { get; set; }

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public decimal? ProcessingFeePercent { get; set; }

    public decimal? ProcessingFeeCgstpercent { get; set; }

    public decimal? ProcessingFeeSgstpercent { get; set; }

    public decimal? ProcessingFeeIgstpercent { get; set; }

    public decimal? InspectionChargePercent { get; set; }

    public decimal? MortalityCoveragePercent { get; set; }

    public decimal? CersaiCharge { get; set; }

    public decimal? CersaiCgstpercent { get; set; }

    public decimal? CersaiSgstpercent { get; set; }

    public decimal? DocumentCharge { get; set; }

    public DateOnly EffectedFrom { get; set; }

    public virtual TblLoanProduct LoanProduct { get; set; } = null!;
}
