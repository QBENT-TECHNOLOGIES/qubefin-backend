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

    // True when HR is genuinely scheduled on this candidate's panel as an interviewer (not just holding the
    // administrative HR row). When true, the frontend should disable the fields shared with the interviewer
    // assessment form (the rating block, IsRecommendedForPosition, PositiveRemarks, NegativeRemarks,
    // AnyOtherJobsSuitedRemarks) - they are HR's own already-submitted interviewer answers, sourced below,
    // and are never re-asked or overwritten by the HR Assessment save/submit endpoints. The remaining
    // decision fields (OverallPerformance onward) stay editable and can be saved/submitted repeatedly,
    // since they no longer share a lock with HR's own interviewer row.
    bool HrIsInterviewer,

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
