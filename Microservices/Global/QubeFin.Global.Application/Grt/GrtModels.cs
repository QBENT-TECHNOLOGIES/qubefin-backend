using FluentValidation;

namespace QubeFin.Global.Application.Grt;

public static class GrtStatuses
{
    public const string InProgress = "In Progress";
    public const string Completed = "Completed";
}

// The 8 key components of the Group Recognition Test, with their check-points, as printed in the document.
// Only the component number is stored in the database, so the wording can change here without touching data.
public static class GrtComponents
{
    public record Component(int No, string Title, string[] Points);

    public static readonly Component[] All =
    [
        new(1, "Group Readiness",
        [
            "Verify successful completion of CGT.",
            "Ensure all members are present and actively participate."
        ]),
        new(2, "Loan Policy Awareness",
        [
            "Assess understanding of loan eligibility, application, approval, repayment schedule, interest rate, other upfront charges.",
            "Ensure members understand the importance of timely repayment and consequences of default."
        ]),
        new(3, "Group Responsibility",
        [
            "Verify understanding of Joint Liability Group (JLG) principles.",
            "Confirm awareness of members' and leaders' roles, meeting discipline, and attendance requirements."
        ]),
        new(4, "Financial Literacy",
        [
            "Assess knowledge of responsible borrowing, household budgeting, savings, and risks of over-indebtedness.",
            "Ensure awareness of client protection principles."
        ]),
        new(5, "Group Meeting Process",
        [
            "Observe a mock group meeting.",
            "Verify proper meeting conduct, member participation, and collective decision-making."
        ]),
        new(6, "Loan Utilization",
        [
            "Confirm understanding that loans must be used for approved activities.",
            "Explain the Loan Utilization Check (LUC) process."
        ]),
        new(7, "Conflict Resolution",
        [
            "Assess the group's ability to resolve disputes and make collective decisions.",
            "Ensure members know how to raise concerns within the group."
        ]),
        new(8, "Grievance Redressal",
        [
            "Verify awareness of WeGrow's grievance redressal mechanism.",
            "Ensure members know their rights and the channels for lodging complaints."
        ])
    ];
}

// The dropdown choices of the Final Group Assessment (the same values are checked here and kept on the screen).
public static class GrtOptions
{
    public static readonly string[] SatisfactoryUnsatisfactory = ["Satisfactory", "Unsatisfactory"];
    public static readonly string[] GoodAveragePoor = ["Good", "Average", "Poor"];
    public static readonly string[] AdequateInadequate = ["Adequate", "Inadequate"];
    public static readonly string[] PassFail = ["Pass", "Fail"];
}

public class GrtComponentRequest
{
    public int ComponentNo { get; set; }
    // null = not answered yet
    public bool? Verified { get; set; }
    public string? Remarks { get; set; }
}

// The Final Group Assessment: null = not answered yet.
public class GrtAssessment
{
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

    // The ten rating fields with the choices each one accepts, in the order of the document.
    public IEnumerable<(string Label, string? Value, string[] Choices)> Ratings() =>
    [
        ("Group Discipline", GroupDiscipline, GrtOptions.SatisfactoryUnsatisfactory),
        ("Participation of Members", ParticipationOfMembers, GrtOptions.GoodAveragePoor),
        ("Leadership Quality", LeadershipQuality, GrtOptions.GoodAveragePoor),
        ("Understanding of Loan Policies", UnderstandingOfLoanPolicies, GrtOptions.AdequateInadequate),
        ("Knowledge of Joint Liability", KnowledgeOfJointLiability, GrtOptions.AdequateInadequate),
        ("Financial Literacy", FinancialLiteracy, GrtOptions.AdequateInadequate),
        ("Client Protection Awareness", ClientProtectionAwareness, GrtOptions.AdequateInadequate),
        ("Meeting Conduct", MeetingConduct, GrtOptions.SatisfactoryUnsatisfactory),
        ("Loan Utilization Awareness", LoanUtilizationAwareness, GrtOptions.AdequateInadequate),
        ("Grievance Awareness", GrievanceAwareness, GrtOptions.AdequateInadequate)
    ];
}

// What the Branch Manager saves. Submit = true also completes the GRT (it is then locked).
public class GrtRequest
{
    public DateOnly GrtDate { get; set; }
    public List<GrtComponentRequest> Components { get; set; } = new();
    public GrtAssessment Assessment { get; set; } = new();
    public bool Submit { get; set; }
}

public class GrtRequestValidator : AbstractValidator<GrtRequest>
{
    public GrtRequestValidator()
    {
        RuleFor(v => v.GrtDate).NotEqual(default(DateOnly)).WithMessage("GRT Date is required.");
        RuleForEach(v => v.Components).ChildRules(c =>
        {
            c.RuleFor(x => x.ComponentNo).InclusiveBetween(1, GrtComponents.All.Length).WithMessage("Component number is not valid.");
            c.RuleFor(x => x.Remarks).MaximumLength(500).WithMessage("Remarks can be 500 characters at most.");
        });
        RuleFor(v => v.Components).Must(c => c.Select(x => x.ComponentNo).Distinct().Count() == c.Count)
            .WithMessage("A component is repeated.");
        RuleFor(v => v.Assessment).NotNull().WithMessage("The assessment is required.");
        // A saved answer must be one of the choices of its dropdown.
        RuleFor(v => v.Assessment).Must(a => a.Ratings().All(r => r.Value is null || r.Choices.Contains(r.Value)))
            .When(v => v.Assessment != null).WithMessage("An assessment answer is not one of the choices.");
        RuleFor(v => v.Assessment.OverallRecommendation)
            .Must(o => o is null || GrtOptions.PassFail.Contains(o))
            .When(v => v.Assessment != null).WithMessage("Overall Recommendation must be Pass or Fail.");
    }
}

public class GrtComponentItem
{
    public int ComponentNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string[] Points { get; set; } = [];
    public bool? Verified { get; set; }
    public string? Remarks { get; set; }
}

public class GrtConductedBy
{
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    // PIN = employee code
    public string? Pin { get; set; }
}

public class GrtResponse
{
    public Guid SurveyId { get; set; }
    // False until the Group Survey is Approved.
    public bool IsOpen { get; set; }
    // The GRT starts only after the CGT is Completed.
    public bool CgtCompleted { get; set; }
    // The Head of the branch can write it, once the CGT is Completed, until it is Completed itself.
    public bool CanEdit { get; set; }
    public bool IsSaved { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string ProposedArea { get; set; } = string.Empty;
    public DateOnly? GrtDate { get; set; }
    public DateTime? CompletedOn { get; set; }
    public GrtConductedBy? ConductedBy { get; set; }
    public List<GrtComponentItem> Components { get; set; } = new();
    public GrtAssessment Assessment { get; set; } = new();
}
