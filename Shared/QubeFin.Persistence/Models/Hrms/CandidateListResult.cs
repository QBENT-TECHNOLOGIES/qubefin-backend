namespace QubeFin.Persistence.Models.Hrms;

/// <summary>One row of Hrms.USP_GetCandidateList (Candidate page list).</summary>
public class CandidateListResult
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? ReferenceNo { get; set; }
    public string? InterviewPost { get; set; }
    public string? CompanyName { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public TimeOnly? InterviewTime { get; set; }
    public string? RecommendationStatus { get; set; }
    public string? Status { get; set; }
    public string? CvFile { get; set; }
    public string? JobApplicationFile { get; set; }
    public string? WrittenInterviewFile { get; set; }
    public string? CreditBureauReportLink { get; set; }
    public string? SignedJoiningLetterFile { get; set; }
    public bool CanSchedule { get; set; }
    public int TotalRecords { get; set; }
}

/// <summary>One row of Hrms.USP_GetInterviewerCandidateList (Interview page list - the caller's own panels).</summary>
public class InterviewerCandidateListResult
{
    public Guid PanelId { get; set; }
    public Guid CandidateId { get; set; }
    public string? FullName { get; set; }
    public string? ReferenceNo { get; set; }
    public string? InterviewPost { get; set; }
    public string? CompanyName { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public TimeOnly? InterviewTime { get; set; }
    public string? CvFile { get; set; }
    public string? Status { get; set; }
    public bool IsCandidateAbsent { get; set; }
    public string? AttenedRemarks { get; set; }
    public bool CanAcknowledge { get; set; }
    public bool CanStartAssessment { get; set; }
    public bool CanContinueAssessment { get; set; }
    public bool CanViewAssessment { get; set; }
    public int TotalRecords { get; set; }
}
