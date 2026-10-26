namespace QubeFin.Hrms.Application.InterviewProcess.Models
{
    public class InterviewerCandidateSearchParam
    {
        public string? SearchText { get; set; }
        public string? Status { get; set; }
        public DateOnly? InterviewDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public record InterviewerCandidateDto(
        Guid PanelId,
        Guid CandidateId,
        string FullName,
        string? ReferenceNo,
        string? InterviewPost,
        string? CompanyName,
        DateOnly? ApplicationDate,
        DateOnly? InterviewDate,
        TimeOnly? InterviewTime,
        string Status,
        bool IsCandidateAbsent,
        string? AttenedRemarks,
        bool CanAcknowledge,
        bool CanStartAssessment,
        bool CanContinueAssessment,
        bool CanViewAssessment,
        string? CvFileUrl,
        string? WrittenInterviewFileUrl);
}
