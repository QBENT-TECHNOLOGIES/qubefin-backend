using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMemberDocument
{
    public Guid Id { get; set; }

    public Guid? MemberId { get; set; }

    public Guid? CoBorrowerId { get; set; }

    public Guid KycDocumentId { get; set; }

    public string DocumentNo { get; set; } = null!;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? FontSideFileName { get; set; }

    public string? FontSideFileSequence { get; set; }

    public string? BackSideFileName { get; set; }

    public string? BackSideFileSequence { get; set; }

    public bool IsVerify { get; set; }

    public string? Remarks { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblCoBorrower? CoBorrower { get; set; }

    public virtual TblKycDocument KycDocument { get; set; } = null!;

    public virtual TblMember? Member { get; set; }
}
