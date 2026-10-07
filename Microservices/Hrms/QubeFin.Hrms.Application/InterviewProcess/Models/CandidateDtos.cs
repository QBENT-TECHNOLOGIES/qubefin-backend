using Microsoft.AspNetCore.Http;
using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public class CandidateSearchParam : SearchParam
{
    public Guid? CompanyId { get; set; }
    public DateOnly? ApplicationDateFrom { get; set; }
    public DateOnly? ApplicationDateTo { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public string? RecommendationStatus { get; set; }
    public string? Status { get; set; }
}

public class CandidateListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ReferenceNo { get; set; }
    public string? InterviewPost { get; set; }
    public string? CompanyName { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public DateOnly? InterviewDate { get; set; }
    public TimeOnly? InterviewTime { get; set; }
    public string RecommendationStatus { get; set; } = "Pending";
    public string Status { get; set; } = CandidateInterviewStatus.SchedulePending;
    public bool CanSchedule { get; set; }
    public List<CandidateDownloadFileDto> Downloads { get; set; } = [];
}

public class CandidateDownloadFileDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public static class CandidateInterviewStatus
{
    public const string SchedulePending = "Schedule Pending";
    public const string ScheduledLetterNotSent = "Interview Scheduled but Letter not sent";
    public const string ScheduledLetterSent = "Interview Scheduled & Letter sent";
    public const string InterviewInProgress = "Interview in Progress";
    public const string HrAssessmentPending = "HR Assessment Pending";
    public const string SelectionPending = "Selection Pending";
    public const string VerificationInProgress = "Candidate Verification in Progress";
    public const string JoiningInProgress = "Joining in Progress";
    public const string Joined = "Joined";
    public const string Rejected = "Rejected";
    public const string NotSelected = "Not Selected";
}
public record CandidateScheduleRequest(DateOnly InterviewDate, TimeOnly InterviewTime);
public class CandidateLetterStatusRequest
{
    public IFormFile? File { get; set; }
    public bool? IsInterviewLetterReceived { get; set; }
    public bool? IsOfferLetterReceived { get; set; }
    public bool? IsAppointmentLetterReceived { get; set; }
    public bool? IsWelcomeLetterReceived { get; set; }
}
public record CandidateVerificationUpdateRequest(
    bool IsAadharValidated,
    bool IsVoterValited,
    bool IsPanValidated,
    bool IsMobileValidated,
    bool IsUanVerified,
    bool IsCreditBureauChecked,
    string? CreditBureauReportLink);

public record CandidateInterviewModeUpdateRequest(string InterviewMode);
public class CandidateInterviewUploadRequest
{
    public IFormFile? Attachment { get; set; }
}

public class CandidateJoiningLetterUploadRequest
{
    public IFormFile? Attachment { get; set; }
}

public record CandidateJoiningLetterStatusDto(bool IsUploaded, string? FileUrl);

public class CandidateCreateUpdateDto
{
    public Guid CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? FatherName { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public string? Email { get; set; }

    public string? HouseNo { get; set; }
    public string? RoadName { get; set; }
    public string? LandMark { get; set; }
    public Guid? AdministrativeUnitId { get; set; }
    public Guid? PoliceStationId { get; set; }
    public Guid? PostOfficeId { get; set; }
    public string? PinCode { get; set; }
    public string? Address { get; set; }
    public IFormFile? CvFile { get; set; }
    public IFormFile? JobApplicationFile { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid InterviewPost { get; set; }
    public Guid? VenueOrganizationUnitId { get; set; }
    public string? InterviewMode { get; set; }
    public string? ReferedBy { get; set; }
    public string? RecruitmentSource { get; set; }
    public string? VacancyReference { get; set; }

    public decimal? CurrentSalary { get; set; }
    public decimal? ExpectedSalary { get; set; }
    public int? NoticePeriodInDays { get; set; }
    public DateOnly? EarliestJoiningDate { get; set; }
    public bool IsWillingRelocate { get; set; }
    public string? PreferredLocation { get; set; }
    public Guid? PostedOrganizationUnitId { get; set; }
    public DateOnly? DateOfJoining { get; set; }
    public TimeOnly? ReportingTime { get; set; }
    public decimal? MonthlyCostCompany { get; set; }

    public string? OverallPerformance { get; set; }
    public string? SuitableRoleDepartment { get; set; }
    public Guid? RecommendedGradeId { get; set; }
    public bool IsTrainingRequired { get; set; }
    public string? RecommendationStatus { get; set; }

    public string? AadharNumber { get; set; }
    public string? VoterNumber { get; set; }
    public string? Pan { get; set; }
    public string? Uan { get; set; }
}
