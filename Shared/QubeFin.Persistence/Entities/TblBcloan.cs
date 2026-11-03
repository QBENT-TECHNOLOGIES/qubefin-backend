using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcloan
{
    public Guid Id { get; set; }

    public string LoanNo { get; set; } = null!;

    public Guid BcmemberId { get; set; }

    public string? LoanProductName { get; set; }

    public string? LoanPurpose { get; set; }

    public decimal? LoanAmount { get; set; }

    public decimal? RateOfInterest { get; set; }

    public int? LoanTenure { get; set; }

    public decimal? Emiamount { get; set; }

    public DateOnly? DisburseDate { get; set; }

    public decimal? DisburseAmount { get; set; }

    public decimal? TotalPaidAmount { get; set; }

    public int? TotalPaidInstallment { get; set; }

    public decimal? CurrentOutstadingAmount { get; set; }

    public decimal? OverdueAmount { get; set; }

    public int? Dpddays { get; set; }

    public bool IsClosed { get; set; }

    public DateOnly? ClosedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblBcmember Bcmember { get; set; } = null!;

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblUser? ModifiedByNavigation { get; set; }
}
