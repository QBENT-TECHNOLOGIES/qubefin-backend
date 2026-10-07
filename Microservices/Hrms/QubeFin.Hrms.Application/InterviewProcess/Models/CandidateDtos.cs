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

    /// <summary>One of <see cref="CandidateInterviewStatus"/>.</summary>
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

    /// <summary>Where the candidate is in the workflow - one of <see cref="CandidateInterviewStatus"/>.</summary>
    public string Status { get; set; } = CandidateInterviewStatus.SchedulePending;

    /// <summary>HR/Admin may add or move the interview date/time (until the HR Assessment is completed).</summary>
    public bool CanSchedule { get; set; }

    /// <summary>Files the list offers for download at the candidate's current <see cref="Status"/>.</summary>
    public List<CandidateDownloadFileDto> Downloads { get; set; } = [];
}

public class CandidateDownloadFileDto
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

/// <summary>Candidate list statuses, derived from the workflow by Hrms.USP_GetCandidateList (keep the two in step).
/// The workflow stops at <see cref="Rejected"/> or <see cref="NotSelected"/>.</summary>
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

/// <summary>Body of the Schedule action: adds or moves the candidate's interview date and time.</summary>
public record CandidateScheduleRequest(DateOnly InterviewDate, TimeOnly InterviewTime);

/// <summary>Send exactly one of these flags (non-null) per request - the rest should be left null.
/// A class with settable properties rather than a positional record on purpose: [FromForm] complex-type
/// binding writes through property setters and needs a parameterless constructor, so a positional record
/// never binds from multipart and every send-letter call comes back 400.</summary>
public class CandidateLetterStatusRequest
{
    public IFormFile? File { get; set; }
    public bool? IsInterviewLetterReceived { get; set; }
    public bool? IsOfferLetterReceived { get; set; }
    public bool? IsAppointmentLetterReceived { get; set; }
    public bool? IsWelcomeLetterReceived { get; set; }
}

/// <summary>All six verification flags are sent together in one call - unlike <see cref="CandidateLetterStatusRequest"/>.</summary>
public record CandidateVerificationUpdateRequest(
    bool IsAadharValidated,
    bool IsVoterValited,
    bool IsPanValidated,
    bool IsMobileValidated,
    bool IsUanVerified,
    bool IsCreditBureauChecked,
    string? CreditBureauReportLink);

public record CandidateInterviewModeUpdateRequest(string InterviewMode);

/// <summary>[FromForm] request body for uploading the candidate's written-interview/personality form.</summary>
public class CandidateInterviewUploadRequest
{
    public IFormFile? Attachment { get; set; }
}

/// <summary>[FromForm] request body for uploading the candidate's signed/returned joining letter.</summary>
public class CandidateJoiningLetterUploadRequest
{
    public IFormFile? Attachment { get; set; }
}

/// <summary>Whether the candidate's signed joining letter has been uploaded, and a URL to download it if so.</summary>
public record CandidateJoiningLetterStatusDto(bool IsUploaded, string? FileUrl);

/// <summary>[FromForm] body for creating / updating a candidate. A class with settable properties (not a
/// positional record) so multipart binding works. Interview date/time are set by the Schedule action instead.</summary>
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

    /// <summary>Mandatory when creating; on update only sent when HR replaces the file.</summary>
    public IFormFile? CvFile { get; set; }

    /// <summary>Mandatory when creating; on update only sent when HR replaces the file.</summary>
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
