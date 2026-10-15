namespace QubeFin.Hrms.Application.InterviewProcess.Models;

// Panel Tagging: a "panel" is not stored on its own. It is the set of candidates who share the same
// interview date, venue organization unit and exact set of panelists (Tbl_InterviewPanel rows).

public class InterviewPanelBoardSearchParam
{
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public Guid? VenueOrganizationUnitId { get; set; }
    public Guid? CompanyId { get; set; }
}

/// <summary>All panels and untagged candidates for one interview date at one venue.</summary>
public record InterviewPanelSlotDto(
    DateOnly InterviewDate,
    Guid VenueOrganizationUnitId,
    string VenueName,
    bool IsPast,
    List<InterviewPanelGroupDto> Panels,
    List<PanelCandidateDto> UntaggedCandidates);

public record InterviewPanelGroupDto(
    string PanelKey,
    List<PanelMemberDto> Members,
    List<PanelCandidateDto> Candidates);

public record PanelMemberDto(Guid EmployeeId, string EmployeeName, string EmployeeCode, string? Designation);

public record PanelCandidateDto(
    Guid CandidateId,
    string FullName,
    string? ReferenceNo,
    string? InterviewPost,
    TimeOnly? InterviewTime,
    int PanelistCount,
    int AcknowledgedCount,
    int SubmittedCount,
    bool IsLocked,
    string? LockedReason);

/// <summary>Multipart body. Acknowledgement PDFs are read from the "Files" form field, each named "{candidateId}.pdf".</summary>
public class InterviewPanelAssignRequest
{
    public List<Guid> CandidateIds { get; set; } = [];
    public List<Guid> EmployeeIds { get; set; } = [];
}

public record InterviewPanelUntagRequest(List<Guid> CandidateIds);
