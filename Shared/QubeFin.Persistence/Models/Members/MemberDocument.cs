namespace QubeFin.Persistence.Models.Members;

public class MemberDocument
{
    public Guid Id { get; private set; }
    public Guid? MemberId { get; private set; }
    public Guid? CoBorrowerId { get; private set; }
    public Guid KycDocumentId { get; private set; }
    public string DocumentNo { get; private set; } = null!;
    public DateOnly? IssueDate { get; private set; }
    public DateOnly? ExpiryDate { get; private set; }
    public string? FontSideFileName { get; private set; }
    public string? FontSideFileSequence { get; private set; }
    public string? BackSideFileName { get; private set; }
    public string? BackSideFileSequence { get; private set; }
    public bool IsVerify { get; private set; }
    public string? Remarks { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    public MemberDocument(
        Guid id,
        Guid? memberId,
        Guid? coBorrowerId,
        Guid kycDocumentId,
        string documentNo,
        DateOnly? issueDate,
        DateOnly? expiryDate,
        string? fontSideFileName,
        string? fontSideFileSequence,
        string? backSideFileName,
        string? backSideFileSequence,
        bool isVerify,
        string? remarks,
        Guid createdBy,
        DateTime createdOn,
        Guid? lastModifiedBy,
        DateTime? lastModifiedOn)
    {
        Id = id;
        MemberId = memberId;
        CoBorrowerId = coBorrowerId;
        KycDocumentId = kycDocumentId;
        DocumentNo = documentNo;
        IssueDate = issueDate;
        ExpiryDate = expiryDate;
        FontSideFileName = fontSideFileName;
        FontSideFileSequence = fontSideFileSequence;
        BackSideFileName = backSideFileName;
        BackSideFileSequence = backSideFileSequence;
        IsVerify = isVerify;
        Remarks = remarks;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }
}