using Microsoft.AspNetCore.Http;

namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public record PanelistScheduleDto(Guid EmployeeId, DateOnly ScheduledDate, TimeOnly ScheduledTime);

/// <summary>Multipart body for scheduling / adding panelists: the panelists plus the interview panel acknowledgement
/// PDF emailed to each of them.</summary>
public class InterviewPanelScheduleRequest
{
    public Guid CandidateId { get; set; }
    public List<PanelistScheduleFormDto> Panelists { get; set; } = [];
    public IFormFile? File { get; set; }

    public List<PanelistScheduleDto> ToPanelists() => Panelists.Select(p => new PanelistScheduleDto(p.EmployeeId, p.ScheduledDate, p.ScheduledTime)).ToList();
}

public class PanelistScheduleFormDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
}

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

/// <summary>Full assessment detail for a single panelist against a candidate, returned by CandidateId + EmployeeId.</summary>
public record InterviewAssessmentDto(
    Guid Id,
    Guid CandidateId,
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string Designation,
    bool IsAcknowledged,
    DateTime? AcknowledgedDate,
    bool IsAttened,
    bool IsSubmitted,
    DateTime? SubmissionDate,
    int? AppearanceAttitudeRating, string? AppearanceAttitudeRemarks,
    int? PersonalityRating, string? PersonalityRemarks,
    int? CommunicationRating, string? CommunicationRemarks,
    int? EducationRating, string? EducationRemarks,
    int? WorkExperienceRating, string? WorkExperienceRemarks,
    int? TechnicalCompetenceRating, string? TechnicalCompetenceRemarks,
    int? FlexibilityRating, string? FlexibilityRemarks,
    int? AmbitionRating, string? AmbitionRemarks,
    int? PotentialRating, string? PotentialRemarks,
    int? OthersRating, string? OthersRemarks,
    int? TotalRatingPoint,
    string? AnyOtherJobsSuitedRemarks,
    bool? IsRecommendedForPosition,
    string? PositiveRemarks,
    string? NegativeRemarks);