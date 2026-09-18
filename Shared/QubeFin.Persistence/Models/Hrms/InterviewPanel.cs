namespace QubeFin.Persistence.Models.Hrms;

public class InterviewPanel
{
    public Guid Id { get; private set; }
    public Guid CandidateId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateOnly ScheduledDate { get; private set; }
    public TimeOnly ScheduledTime { get; private set; }
    public bool IsAcknowledged { get; private set; }
    public DateTime? AcknowledgedDate { get; private set; }
    public bool IsAttened { get; private set; }

    public int? AppearanceAttitudeRating { get; private set; }
    public string? AppearanceAttitudeRemarks { get; private set; }
    public int? PersonalityRating { get; private set; }
    public string? PersonalityRemarks { get; private set; }
    public int? CommunicationRating { get; private set; }
    public string? CommunicationRemarks { get; private set; }
    public int? EducationRating { get; private set; }
    public string? EducationRemarks { get; private set; }
    public int? WorkExperienceRating { get; private set; }
    public string? WorkExperienceRemarks { get; private set; }
    public int? TechnicalCompetenceRating { get; private set; }
    public string? TechnicalCompetenceRemarks { get; private set; }
    public int? FlexibilityRating { get; private set; }
    public string? FlexibilityRemarks { get; private set; }
    public int? AmbitionRating { get; private set; }
    public string? AmbitionRemarks { get; private set; }
    public int? PotentialRating { get; private set; }
    public string? PotentialRemarks { get; private set; }
    public int? OthersRating { get; private set; }
    public string? OthersRemarks { get; private set; }

    public string? AnyOtherJobsSuitedRemarks { get; private set; }
    public bool? IsRecommendedForPosition { get; private set; }
    public string? PositiveRemarks { get; private set; }
    public string? NegativeRemarks { get; private set; }

    public bool IsSubmitted { get; private set; }
    public DateTime? SubmissionDate { get; private set; }
    public Guid ModifiedBy { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public string? Designation { get; private set; } = null;
    public string? EmployeeName { get; private set; } = null;
    public string? EmployeeCode { get; private set; } = null;

    /// <summary>Sum of the ten scored parameters. Null until at least one has been rated.</summary>
    public int? TotalRatingPoint
    {
        get
        {
            var ratings = new[]
            {
                AppearanceAttitudeRating, PersonalityRating, CommunicationRating, EducationRating,
                WorkExperienceRating, TechnicalCompetenceRating, FlexibilityRating, AmbitionRating,
                PotentialRating, OthersRating
            };

            return ratings.Any(r => r.HasValue)
                ? ratings.Where(r => r.HasValue).Sum(r => r!.Value)
                : null;
        }
    }

    private InterviewPanel() { }

    // Full reconstruction constructor - used by the mapper when hydrating from persistence.
    public InterviewPanel(
        Guid id,
        Guid candidateId,
        Guid employeeId,
        DateOnly scheduledDate,
        TimeOnly scheduledTime,
        bool isAcknowledged,
        DateTime? acknowledgedDate,
        bool isAttened,
        int? appearanceAttitudeRating,
        string? appearanceAttitudeRemarks,
        int? personalityRating,
        string? personalityRemarks,
        int? communicationRating,
        string? communicationRemarks,
        int? educationRating,
        string? educationRemarks,
        int? workExperienceRating,
        string? workExperienceRemarks,
        int? technicalCompetenceRating,
        string? technicalCompetenceRemarks,
        int? flexibilityRating,
        string? flexibilityRemarks,
        int? ambitionRating,
        string? ambitionRemarks,
        int? potentialRating,
        string? potentialRemarks,
        int? othersRating,
        string? othersRemarks,
        string? anyOtherJobsSuitedRemarks,
        bool? isRecommendedForPosition,
        string? positiveRemarks,
        string? negativeRemarks,
        bool isSubmitted,
        DateTime? submissionDate,
        Guid modifiedBy,
        DateTime modifiedOn,
         string? employeeCode = null,
         string? designation = null,
         string? employeeName = null)
    {
        Id = id;
        CandidateId = candidateId;
        EmployeeId = employeeId;
        ScheduledDate = scheduledDate;
        ScheduledTime = scheduledTime;
        IsAcknowledged = isAcknowledged;
        AcknowledgedDate = acknowledgedDate;
        IsAttened = isAttened;
        AppearanceAttitudeRating = appearanceAttitudeRating;
        AppearanceAttitudeRemarks = appearanceAttitudeRemarks;
        PersonalityRating = personalityRating;
        PersonalityRemarks = personalityRemarks;
        CommunicationRating = communicationRating;
        CommunicationRemarks = communicationRemarks;
        EducationRating = educationRating;
        EducationRemarks = educationRemarks;
        WorkExperienceRating = workExperienceRating;
        WorkExperienceRemarks = workExperienceRemarks;
        TechnicalCompetenceRating = technicalCompetenceRating;
        TechnicalCompetenceRemarks = technicalCompetenceRemarks;
        FlexibilityRating = flexibilityRating;
        FlexibilityRemarks = flexibilityRemarks;
        AmbitionRating = ambitionRating;
        AmbitionRemarks = ambitionRemarks;
        PotentialRating = potentialRating;
        PotentialRemarks = potentialRemarks;
        OthersRating = othersRating;
        OthersRemarks = othersRemarks;
        AnyOtherJobsSuitedRemarks = anyOtherJobsSuitedRemarks;
        IsRecommendedForPosition = isRecommendedForPosition;
        PositiveRemarks = positiveRemarks;
        NegativeRemarks = negativeRemarks;
        IsSubmitted = isSubmitted;
        SubmissionDate = submissionDate;
        ModifiedBy = modifiedBy;
        ModifiedOn = modifiedOn;
        EmployeeCode = employeeCode;
        EmployeeName = employeeName;
        Designation = designation;
    }

    /// <summary>HR schedules a panelist against a candidate's interview.</summary>
    public static InterviewPanel Schedule(
        Guid candidateId,
        Guid employeeId,
        DateOnly scheduledDate,
        TimeOnly scheduledTime,
        Guid scheduledBy)
    {
        return new InterviewPanel
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            EmployeeId = employeeId,
            ScheduledDate = scheduledDate,
            ScheduledTime = scheduledTime,
            IsAcknowledged = false,
            IsAttened = false,
            IsSubmitted = false,
            ModifiedBy = scheduledBy,
            ModifiedOn = DateTime.UtcNow
        };
    }

    /// <summary>Panelist confirms they will attend. Idempotent - a second call is a no-op.</summary>
    public void Acknowledge(Guid acknowledgedBy)
    {
        if (IsAcknowledged)
        {
            return;
        }

        IsAcknowledged = true;
        AcknowledgedDate = DateTime.UtcNow;
        ModifiedBy = acknowledgedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Marks whether the panelist actually showed up on interview day.</summary>
    public void MarkAttendance(bool attended, Guid modifiedBy)
    {
        IsAttened = attended;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>Panelist submits their scored assessment. Locked once submitted - resubmission fails.</summary>
    public bool SubmitAssessment(AssessmentDetails details, Guid submittedBy)
    {
        if (IsSubmitted)
        {
            return false;
        }

        AppearanceAttitudeRating = details.AppearanceAttitudeRating;
        AppearanceAttitudeRemarks = details.AppearanceAttitudeRemarks;
        PersonalityRating = details.PersonalityRating;
        PersonalityRemarks = details.PersonalityRemarks;
        CommunicationRating = details.CommunicationRating;
        CommunicationRemarks = details.CommunicationRemarks;
        EducationRating = details.EducationRating;
        EducationRemarks = details.EducationRemarks;
        WorkExperienceRating = details.WorkExperienceRating;
        WorkExperienceRemarks = details.WorkExperienceRemarks;
        TechnicalCompetenceRating = details.TechnicalCompetenceRating;
        TechnicalCompetenceRemarks = details.TechnicalCompetenceRemarks;
        FlexibilityRating = details.FlexibilityRating;
        FlexibilityRemarks = details.FlexibilityRemarks;
        AmbitionRating = details.AmbitionRating;
        AmbitionRemarks = details.AmbitionRemarks;
        PotentialRating = details.PotentialRating;
        PotentialRemarks = details.PotentialRemarks;
        OthersRating = details.OthersRating;
        OthersRemarks = details.OthersRemarks;
        AnyOtherJobsSuitedRemarks = details.AnyOtherJobsSuitedRemarks;
        IsRecommendedForPosition = details.IsRecommendedForPosition;
        PositiveRemarks = details.PositiveRemarks;
        NegativeRemarks = details.NegativeRemarks;

        IsSubmitted = true;
        SubmissionDate = DateTime.UtcNow;
        ModifiedBy = submittedBy;
        ModifiedOn = DateTime.UtcNow;

        return true;
    }
}

public record AssessmentDetails(
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