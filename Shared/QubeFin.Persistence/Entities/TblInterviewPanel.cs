using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblInterviewPanel
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public bool IsAcknowledged { get; set; }

    public DateTime? AcknowledgedDate { get; set; }

    public bool IsAttened { get; set; }

    public int AppearanceAttitudeRating { get; set; }

    public string? AppearanceAttitudeRemarks { get; set; }

    public int PersonalityRating { get; set; }

    public string? PersonalityRemarks { get; set; }

    public int CommunicationRating { get; set; }

    public string? CommunicationRemarks { get; set; }

    public int EducationRating { get; set; }

    public string? EducationRemarks { get; set; }

    public int WorkExperienceRating { get; set; }

    public string? WorkExperienceRemarks { get; set; }

    public int TechnicalCompetenceRating { get; set; }

    public string? TechnicalCompetenceRemarks { get; set; }

    public int FlexibilityRating { get; set; }

    public string? FlexibilityRemarks { get; set; }

    public int AmbitionRating { get; set; }

    public string? AmbitionRemarks { get; set; }

    public int PotentialRating { get; set; }

    public string? PotentialRemarks { get; set; }

    public int OthersRating { get; set; }

    public string? OthersRemarks { get; set; }

    public string? AnyOtherJobsSuitedRemarks { get; set; }

    public bool IsRecommendedForPosition { get; set; }

    public string? PositiveRemarks { get; set; }

    public string? NegativeRemarks { get; set; }

    public bool IsSubmitted { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public virtual TblInterviewCandidate Candidate { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblUser ModifiedByNavigation { get; set; } = null!;
}
