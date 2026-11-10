namespace QubeFin.Persistence.Models.Collection;

/// <summary>One row of the [dbo].[UT_LoanCollection] table type sent to [BC].[USP_BCLoanCollection].</summary>
public sealed class LoanCollectionEntry
{
    public Guid LoanId { get; set; }
    public DateTime CollectionDate { get; set; }
    public decimal CollectionAmount { get; set; }
    public bool IsUpiCollection { get; set; }
    public string? UtrNo { get; set; }
    public string? Remarks { get; set; }
    public Guid UserId { get; set; }
}
