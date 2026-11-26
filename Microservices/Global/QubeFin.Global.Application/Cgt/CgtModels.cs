using FluentValidation;

namespace QubeFin.Global.Application.Cgt;

public static class CgtRules
{
    // The CGT can start only when Community Mobilization has at least this many borrowers (every borrower counts).
    public const int MinBorrowers = 5;
}

public static class CgtStatuses
{
    public const string InProgress = "In Progress";
    public const string Completed = "Completed";
}

// The 10 topics of the Compulsory Group Training, with their sub-points, as printed in the document.
// Only the topic number is stored in the database, so the wording can change here without touching data.
public static class CgtTopics
{
    public record Topic(int No, string Title, string[] Points);

    public static readonly Topic[] All =
    [
        new(1, "Introduction to WeGrow & Microfinance",
        [
            "Explain the mission and objectives of WeGrow.",
            "Provide an overview of microfinance, its benefits, and how it empowers small entrepreneurs and communities."
        ]),
        new(2, "Formation of the Group",
        [
            "Explain the group lending model and its importance.",
            "Define the roles and responsibilities of group members.",
            "Discuss the importance of peer support and joint liability."
        ]),
        new(3, "Roles & Responsibilities of Group Leaders",
        [
            "Election of President, Secretary, and Cashier.",
            "Explain their specific duties and responsibilities in loan management, attendance, and meeting facilitation."
        ]),
        new(4, "Loan Policies & Procedures",
        [
            "Explain the loan eligibility criteria.",
            "Detail the membership, loan application, appraisal and approval process and disbursement.",
            "Clarify repayment schedule, interest rates and other charges."
        ]),
        new(5, "Financial Literacy & Client Protection Principles",
        [
            "Educate members about basic financial management and budgeting.",
            "Raise awareness on over-indebtedness risks and responsible borrowing.",
            "Inform members about RBI regulations and client protection principles."
        ]),
        new(6, "Group Meeting Norms & Attendance Rules",
        [
            "Explain the importance of regular attendance in meetings.",
            "Describe the U-shape seating arrangement for structured discussions.",
            "Highlight the need for respectful communication and disciplined meetings."
        ]),
        new(7, "Loan Utilization Check (LUC) & Follow-ups",
        [
            "Train members on proper loan utilization for applied purposes.",
            "Explain the Loan Utilization Check (LUC) process by Credit Officers."
        ]),
        new(8, "Dispute Resolution & Conflict Management",
        [
            "Teach members how to handle internal conflicts peacefully.",
            "Outline the process for raising concerns and resolving issues within the group."
        ]),
        new(9, "Rights & Responsibilities of Borrowers",
        [
            "Inform members about their rights under WeGrow's policies.",
            "Discuss grievance redressal mechanisms available for members.",
            "Encourage open communication with Credit Officers."
        ]),
        new(10, "Final Assessment & Group Recognition Test (GRT)",
        [
            "Conduct a question-answer session to assess members' understanding.",
            "Ensure that all members agree to the group's terms and policies before moving forward."
        ])
    ];
}

public class CgtTopicRequest
{
    public int TopicNo { get; set; }
    // null = not answered yet
    public bool? Covered { get; set; }
    public string? Remarks { get; set; }
}

// What the Credit Officer saves. Complete = true also marks the CGT Completed (it is then locked).
public class CgtRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<CgtTopicRequest> Topics { get; set; } = new();
    public bool Complete { get; set; }
}

public class CgtRequestValidator : AbstractValidator<CgtRequest>
{
    public CgtRequestValidator()
    {
        RuleFor(v => v.StartDate).NotEqual(default(DateOnly)).WithMessage("Start Date is required.");
        RuleFor(v => v.EndDate).GreaterThanOrEqualTo(v => v.StartDate).When(v => v.EndDate.HasValue)
            .WithMessage("End Date cannot be before the Start Date.");
        RuleForEach(v => v.Topics).ChildRules(t =>
        {
            t.RuleFor(x => x.TopicNo).InclusiveBetween(1, CgtTopics.All.Length).WithMessage("Topic number is not valid.");
            t.RuleFor(x => x.Remarks).MaximumLength(500).WithMessage("Remarks can be 500 characters at most.");
        });
        RuleFor(v => v.Topics).Must(t => t.Select(x => x.TopicNo).Distinct().Count() == t.Count)
            .WithMessage("A topic is repeated.");
    }
}

public class CgtTopicItem
{
    public int TopicNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string[] Points { get; set; } = [];
    public bool? Covered { get; set; }
    public string? Remarks { get; set; }
}

public class CgtConductedBy
{
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? EmployeeCode { get; set; }
}

public class CgtResponse
{
    public Guid SurveyId { get; set; }
    // False until the Group Survey is Approved.
    public bool IsOpen { get; set; }
    // The survey's Credit Officers can write it until it is Completed.
    public bool CanEdit { get; set; }
    // False until a Credit Officer saves it the first time.
    public bool IsSaved { get; set; }
    // Borrowers in Community Mobilization now, and how many are needed before the CGT can start.
    public int BorrowerCount { get; set; }
    public int MinBorrowers { get; set; } = CgtRules.MinBorrowers;
    public string Status { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string ProposedArea { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime? CompletedOn { get; set; }
    public CgtConductedBy? ConductedBy { get; set; }
    public List<CgtTopicItem> Topics { get; set; } = new();
}
