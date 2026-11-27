using Microsoft.AspNetCore.Http;

namespace QubeFin.Global.Application.Members.Models;

public class MemberDocumentsFormRequest
{
    public List<MemberDocumentRequest> Documents { get; set; } = [];
}

public class MemberDocumentRequest
{
    public Guid? Id { get; set; }
    public Guid KycDocumentId { get; set; }
    public string DocumentNo { get; set; } = null!;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public string? FontSideFileName { get; set; }
    public string? FontSideFileSequence { get; set; }
    public IFormFile? FontSideFile { get; set; }

    public string? BackSideFileName { get; set; }
    public string? BackSideFileSequence { get; set; }
    public IFormFile? BackSideFile { get; set; }
    public bool IsVerify { get; set; }
    public string? Remarks { get; set; }
}