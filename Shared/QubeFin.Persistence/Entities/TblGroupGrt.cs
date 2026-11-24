using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupGrt
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public DateOnly GrtDate { get; set; }

    public Guid ConductedByEmployeeId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CompletedOn { get; set; }

    public bool? CgtSuccessfullyCompleted { get; set; }

    public string? GroupDiscipline { get; set; }

    public string? ParticipationOfMembers { get; set; }

    public string? LeadershipQuality { get; set; }

    public string? UnderstandingOfLoanPolicies { get; set; }

    public string? KnowledgeOfJointLiability { get; set; }

    public string? FinancialLiteracy { get; set; }

    public string? ClientProtectionAwareness { get; set; }

    public string? MeetingConduct { get; set; }

    public string? LoanUtilizationAwareness { get; set; }

    public string? GrievanceAwareness { get; set; }

    public string? OverallRecommendation { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee ConductedByEmployee { get; set; } = null!;

    public virtual TblSurvey Survey { get; set; } = null!;

    public virtual ICollection<TblGroupGrtComponent> TblGroupGrtComponents { get; set; } = new List<TblGroupGrtComponent>();
}
