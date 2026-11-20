using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcloanCollection
{
    public Guid Id { get; set; }

    public Guid LoanId { get; set; }

    public decimal EmiAmount { get; set; }

    public DateOnly CollectionDate { get; set; }

    public decimal CollectionAmount { get; set; }

    public decimal? MoreCollection { get; set; }

    public decimal? LessCollection { get; set; }

    public bool IsDemandDate { get; set; }

    public bool IsCollected { get; set; }

    public bool IsUpiCollection { get; set; }

    public string? UtrNo { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? PostedBy { get; set; }

    public DateTime? PostedOn { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblBcloan Loan { get; set; } = null!;
}
