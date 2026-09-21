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

/// <summary>
/// Returned when HR opens the HR Assessment form for a candidate. The ten category ratings are the
/// average of every panelist's submitted score for that category - shown read-only in the form. Below
/// that, OverallPerformance/SuitableRoleDepartment/RecommendedGradeId/IsTrainingRequired/RecommendationStatus
/// are HR's own editable decision fields (pre-filled from a previous draft, if any).
/// </summary>
public record HrAssessmentFormDto(
    Guid CandidateId,
    bool IsSubmitted,

    // True when HR has already submitted their own individual interviewer assessment (genuinely scheduled
    // on the panel, not just holding the administrative HR row). The fields shared with the interviewer
    // assessment form (the rating block, IsRecommendedForPosition, PositiveRemarks, NegativeRemarks,
    // AnyOtherJobsSuitedRemarks) are sourced from HR's own submission below and are never re-asked or
    // overwritten by the HR Assessment save/submit endpoints in either case; whether the frontend should
    // render them disabled or editable is IsHrOnlyInterviewer, right below.
    bool HrIsInterviewer,

    // True when HrIsInterviewer is true AND no other panelist is scheduled on this candidate's panel - HR
    // is literally the sole interviewer. In that case the shared fields should render disabled (HR's own
    // single score is definitive, nothing else to reconcile it against). When false but HrIsInterviewer is
    // true, HR is one of several interviewers - the shared fields (the live average of everyone including
    // HR) keep updating as other panelists submit, so the frontend should leave them enabled.
    bool IsHrOnlyInterviewer,

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

    // Editable by HR (pre-filled with whatever was last saved, if a draft/submission exists).
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,
    string? AnyOtherJobsSuitedRemarks,
    bool? IsRecommendedForPosition,
    string? PositiveRemarks,
    string? NegativeRemarks,

    // For reference - the individual panelists these averages were computed from.
    IReadOnlyList<PanelistRatingSummaryDto> Panelists);

/// <summary>Body for both the HR Assessment draft-save and submit endpoints. The ten category ratings are
/// never sent here - the server always (re)computes them as the average of the submitted panelists'
/// ratings, since they're read-only/disabled in the HR Assessment form.</summary>
public record HrAssessmentDecisionDto(
    string? interviewMode,
    string? OverallPerformance,
    string? SuitableRoleDepartment,
    Guid? RecommendedGradeId,
    bool IsTrainingRequired,
    string? RecommendationStatus,
    string? AnyOtherJobsSuitedRemarks,
    bool? IsRecommendedForPosition,
    string? PositiveRemarks,
    string? NegativeRemarks);
