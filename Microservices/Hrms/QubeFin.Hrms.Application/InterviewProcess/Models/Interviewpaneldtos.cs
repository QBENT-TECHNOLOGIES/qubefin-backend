namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public record PanelistScheduleDto(Guid EmployeeId, DateOnly ScheduledDate, TimeOnly ScheduledTime);

public record AssessmentSubmitDto(
    int? AppearanceAttitudeRating,
    string? AppearanceAttitudeRemarks,
    int? PersonalityRating,
    string? PersonalityRemarks,
    int? CommunicationRating,
    string? CommunicationRemarks,
    int? EducationRating,
    string? EducationRemarks,
    int? WorkExperienceRating,
    string? WorkExperienceRemarks,
    int? TechnicalCompetenceRating,
    string? TechnicalCompetenceRemarks,
    int? FlexibilityRating,
    string? FlexibilityRemarks,
    int? AmbitionRating,
    string? AmbitionRemarks,
    int? PotentialRating,
    string? PotentialRemarks,
    int? OthersRating,
    string? OthersRemarks,
    string? AnyOtherJobsSuitedRemarks,
    bool? IsRecommendedForPosition,
    string? PositiveRemarks,
    string? NegativeRemarks);

public record InterviewPanelDto(
    Guid Id,
    Guid CandidateId,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string designation,
    DateOnly ScheduledDate,
    TimeOnly ScheduledTime,
    bool IsAcknowledged,
    DateTime? AcknowledgedDate,
    bool IsAttened,
    bool IsSubmitted,
    DateTime? SubmissionDate,
    int? TotalRatingPoint,
    bool? IsRecommendedForPosition);