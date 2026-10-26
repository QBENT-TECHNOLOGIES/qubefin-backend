namespace QubeFin.Hrms.Application.InterviewProcess.Models;

public record PanelistRatingSummaryDto(
    Guid EmployeeId,
    string EmployeeCode,
    string EmployeeName,
    string Designation,
    int? AppearanceAttitudeRating,
    int? PersonalityRating,
    int? CommunicationRating,
    int? EducationRating,
    int? WorkExperienceRating,
    int? TechnicalCompetenceRating,
    int? FlexibilityRating,
    int? AmbitionRating,
    int? PotentialRating,
    int? OthersRating,
    int? TotalRatingPoint,
    bool? IsRecommendedForPosition);

public record PendingPanelistDto(Guid EmployeeId, string EmployeeCode, string EmployeeName, bool IsAcknowledged);

public record HrAssessmentFormDto(
    Guid CandidateId,

    bool IsSubmitted,

    int? AverageAppearanceAttitudeRating,
    int? AveragePersonalityRating,
    int? AverageCommunicationRating,
    int? AverageEducationRating,
    int? AverageWorkExperienceRating,
    int? AverageTechnicalCompetenceRating,
    int? AverageFlexibilityRating,
    int? AverageAmbitionRating,
    int? AveragePotentialRating,
    int? AverageOthersRating,
    int? AverageTotalRatingPoint,

    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,

    decimal? CurrentSalary,
    decimal? ExpectedSalary,
    int? NoticePeriodInDays,
    DateOnly? EarliestJoiningDate,
    bool IsWillingRelocate,
    string? PreferredLocation,

    IReadOnlyList<PanelistRatingSummaryDto> Panelists,

    IReadOnlyList<PendingPanelistDto> PendingPanelists);

public record HrAssessmentDecisionDto(
    string? interviewMode,
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,

    decimal? CurrentSalary,
    decimal? ExpectedSalary,
    int? NoticePeriodInDays,
    DateOnly? EarliestJoiningDate,
    bool IsWillingRelocate,
    string? PreferredLocation);
