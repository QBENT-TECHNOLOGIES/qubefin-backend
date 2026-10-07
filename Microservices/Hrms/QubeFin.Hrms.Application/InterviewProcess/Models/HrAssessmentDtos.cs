namespace QubeFin.Hrms.Application.InterviewProcess.Models;

/// <summary>One submitted panelist's ratings, shown for HR's reference alongside the averages.</summary>
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

/// <summary>An interviewer whose assessment is still outstanding when HR opens the HR Assessment. HR is warned
/// and may proceed anyway - the outstanding assessment is left out of the averages.</summary>
public record PendingPanelistDto(Guid EmployeeId, string EmployeeCode, string EmployeeName, bool IsAcknowledged);

/// <summary>
/// Returned when HR opens the HR Assessment form for a candidate. The ten category ratings are the average of
/// every submitted interviewer's score for that category - shown read-only. Below that are HR's own decision
/// fields (pre-filled from a saved draft, if any). Nothing here is stored in Hrms.Tbl_InterviewPanel.
/// </summary>
public record HrAssessmentFormDto(
    Guid CandidateId,

    // IsHrAssessmentCompleted - the form is read-only once true.
    bool IsSubmitted,

    // Read-only: average of the submitted panelists' ratings per category.
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

    // Editable by HR (pre-filled with whatever was last saved).
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,

    // Salary & joining expectations - stored on the candidate; HR confirms/corrects them here.
    decimal? CurrentSalary,
    decimal? ExpectedSalary,
    int? NoticePeriodInDays,
    DateOnly? EarliestJoiningDate,
    bool IsWillingRelocate,
    string? PreferredLocation,

    // For reference - the submitted panelists these averages were computed from.
    IReadOnlyList<PanelistRatingSummaryDto> Panelists,

    // Panelists who have not submitted (and did not record the candidate absent). Non-empty -> warn HR.
    IReadOnlyList<PendingPanelistDto> PendingPanelists);

/// <summary>Body for both the HR Assessment draft-save and submit endpoints. The ten category ratings are
/// never sent here - the server always (re)computes them as the average of the submitted panelists'
/// ratings, since they're read-only in the HR Assessment form.</summary>
public record HrAssessmentDecisionDto(
    string? interviewMode,
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,

    // Salary & joining expectations, written straight to the candidate by both the draft and the submit.
    decimal? CurrentSalary,
    decimal? ExpectedSalary,
    int? NoticePeriodInDays,
    DateOnly? EarliestJoiningDate,
    bool IsWillingRelocate,
    string? PreferredLocation);
