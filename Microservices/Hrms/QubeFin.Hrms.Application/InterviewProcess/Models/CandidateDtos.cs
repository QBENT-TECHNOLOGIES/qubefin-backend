using Microsoft.AspNetCore.Http;
using QubeFin.Persistence.Models;

namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public class CandidateSearchParam : SearchParam
{
    public Guid? CompanyId { get; set; }
}

public class CandidateListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string InterviewPost { get; set; } = string.Empty;
    public DateOnly InterviewDate { get; set; }
    public string? InterviewTime { get; set; }
    public string? RecommendationStatus { get; set; }
    public int? TotalRatingPoint { get; set; }
    public string? RatingStatus { get; set; }
    public string? ReferenceNo { get; set; }
}

public class CandidateDetailDto
{
    public Guid Id { get; set; }

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

    public string? ReferenceNo { get; set; }
    public DateOnly InterviewDate { get; set; }
    public TimeOnly? InterviewTime { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid InterviewPost { get; set; }
    public string? interviewPostName { get; set; }
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
    public int? TotalRatingPoint { get; set; }
    public string? RatingStatus { get; set; }

    public string? AadharNumber { get; set; }
    public bool IsAadharValidated { get; set; }
    public string? VoterNumber { get; set; }
    public bool IsVoterValited { get; set; }
    public string? Pan { get; set; }
    public bool IsPanValidated { get; set; }
    public bool IsMobileValidated { get; set; }
    public string? Uan { get; set; }
    public bool IsUanVerified { get; set; }
    public bool IsCreditBureauChecked { get; set; }
    public string? CreditBureauReportLink { get; set; }
}

/// <summary>Send exactly one of these flags (non-null) per request - the rest should be left null.</summary>
public record CandidateLetterStatusRequest(
    bool? IsInterviewLetterReceived,
    bool? IsOfferLetterReceived,
    bool? IsAppointmentLetterReceived,
    bool? IsWelcomeLetterReceived);

/// <summary>[FromForm] request body for uploading the candidate's written-interview/personality form.</summary>
public class CandidateInterviewUploadRequest
{
    public IFormFile? Attachment { get; set; }
}

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

    public DateOnly InterviewDate { get; set; }
    public TimeOnly? InterviewTime { get; set; }
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
