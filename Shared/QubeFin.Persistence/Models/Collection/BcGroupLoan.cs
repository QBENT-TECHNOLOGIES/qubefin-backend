namespace QubeFin.Persistence.Models.Collection;

/// <summary>One row of the group wise BC loan search; TotalRecords is the row count before paging.</summary>
public sealed class BcGroupLoan
{
    public Guid Id { get; set; }
    public string LoanNo { get; set; } = string.Empty;
    public Guid BcGroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public Guid BcMemberId { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? MemberMobileNo { get; set; }
    public string? LoanProductName { get; set; }
    public string? LoanPurpose { get; set; }
    public decimal? LoanAmount { get; set; }
    public decimal? RateOfInterest { get; set; }
    public int? LoanTenure { get; set; }
    public decimal? EmiAmount { get; set; }
    public DateOnly? DisburseDate { get; set; }
    public decimal? DisburseAmount { get; set; }
    public decimal? TotalPaidAmount { get; set; }
    public int? TotalPaidInstallment { get; set; }
    public decimal? CurrentOutstandingAmount { get; set; }
    public decimal? OverdueAmount { get; set; }
    public int? DpdDays { get; set; }
    public bool IsClosed { get; set; }
    public DateOnly? ClosedDate { get; set; }

    #region --- COLLECTION DATE ---
    // The loan's BC.Tbl_BCLoanCollection row for the collection date asked for. Null / false when the sheet for that
    // date has not been raised yet, so a loan still appears on the sheet with nothing collected against it.
    public Guid? CollectionId { get; set; }
    public decimal? CollectionEmiAmount { get; set; }
    public decimal? CollectedAmount { get; set; }
    public bool IsDemandDate { get; set; }
    public bool IsCollected { get; set; }
    // How many collections were posted for the date. CollectedAmount is their sum.
    public int CollectionCount { get; set; }
    public bool IsUpiCollection { get; set; }
    public string? CollectionUtrNo { get; set; }
    public string? CollectionRemarks { get; set; }
    #endregion

    public int TotalRecords { get; set; }
}
