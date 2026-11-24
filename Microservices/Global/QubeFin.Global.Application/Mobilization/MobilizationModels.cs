using FluentValidation;

namespace QubeFin.Global.Application.Mobilization;

public static class BorrowerStatuses
{
    public const string Pending = "Pending";
    public const string Blocked = "Blocked";
    public const string Rejected = "Rejected";
    public const string MemberCreated = "Member Created";
}

public static class BorrowerDecisions
{
    public const string Add = "Add";
    public const string Block = "Block";
}

// The eligibility rules printed on the Community Mobilization form (R-05). A person who breaks any rule is "flagged":
// the Credit Officer must choose Add or Block and give a reason; the Head of the branch can change the choice.
public static class BorrowerEligibility
{
    public const decimal MaxYearlyIncome = 300000m;   // 3.00 lakh
    public const decimal MaxIndebtedness = 200000m;   // 2.00 lakh

    public static readonly string[] MaritalStatuses = ["Single", "Married", "Divorced", "Widow"];

    // Every breached rule, as text.
    public static List<string> GetBreaches(decimal income, decimal loanAmount, bool hasNpa, bool hasWriteOff)
    {
        var breaches = new List<string>();
        if (hasWriteOff) breaches.Add("Write-off is not Nil");
        if (hasNpa) breaches.Add("NPA is not Nil");
        if (income > MaxYearlyIncome) breaches.Add("Yearly income is more than 3.00 lakh");
        if (loanAmount > MaxIndebtedness) breaches.Add("Indebtedness is more than 2.00 lakh");
        return breaches;
    }
}

// What the Credit Officer types for one prospective borrower.
public class BorrowerRequest
{
    public string Name { get; set; } = string.Empty;
    // Husband / Father
    public string GuardianName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool OwnHouse { get; set; }
    public string? IncomeActivity { get; set; }
    // Current yearly family income and pending loan amount, in rupees.
    public decimal Income { get; set; }
    public decimal LoanAmount { get; set; }
    public bool HasNpa { get; set; }
    public bool HasWriteOff { get; set; }
    // Needed only when a rule is breached: "Add" or "Block", with a reason.
    public string? Decision { get; set; }
    public string? DecisionReason { get; set; }
}

public class BorrowerRequestValidator : AbstractValidator<BorrowerRequest>
{
    public BorrowerRequestValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100).WithMessage("Name can be 100 characters at most.");
        RuleFor(v => v.GuardianName).NotEmpty().WithMessage("Husband / Father name is required.").MaximumLength(100).WithMessage("Husband / Father name can be 100 characters at most.");
        RuleFor(v => v.MobileNo).NotEmpty().WithMessage("Mobile No is required.").Matches(@"^\d{10}$").WithMessage("Mobile No must be 10 digits.");
        RuleFor(v => v.MaritalStatus).Must(m => BorrowerEligibility.MaritalStatuses.Contains(m)).WithMessage("Select the Marital Status.");
        RuleFor(v => v.Age).InclusiveBetween(1, 120).WithMessage("Age must be between 1 and 120.");
        RuleFor(v => v.IncomeActivity).MaximumLength(200).WithMessage("Income-generating activity can be 200 characters at most.");
        RuleFor(v => v.Income).InclusiveBetween(0m, 999999999999m).WithMessage("Income is not valid.");
        RuleFor(v => v.LoanAmount).InclusiveBetween(0m, 999999999999m).WithMessage("Pending loan amount is not valid.");
        RuleFor(v => v.Decision).Must(d => d == null || d == BorrowerDecisions.Add || d == BorrowerDecisions.Block).WithMessage("Decision must be Add or Block.");
        RuleFor(v => v.DecisionReason).MaximumLength(500).WithMessage("Reason can be 500 characters at most.");
    }
}

public class BorrowerItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GuardianName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool OwnHouse { get; set; }
    public string? IncomeActivity { get; set; }
    public decimal Income { get; set; }
    public decimal LoanAmount { get; set; }
    // The two Yes/No fields of the document (stored, set from the amounts):
    // "Yearly Family Household Income <= 3.00 Lakh" and "Indebtedness Cap <= 2.00 Lakh".
    public bool IncomeWithinLimit { get; set; }
    public bool IndebtednessWithinLimit { get; set; }
    public bool HasNpa { get; set; }
    public bool HasWriteOff { get; set; }
    public bool IsFlagged { get; set; }
    public string? FlagReasons { get; set; }
    public string? DecisionReason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectReason { get; set; }
    // Only a Pending or Blocked borrower can still be changed.
    public bool CanChange { get; set; }
}

public class MobilizationResponse
{
    public Guid SurveyId { get; set; }
    public string Status { get; set; } = string.Empty;
    // The survey's Credit Officers add and edit borrowers once the survey is Approved.
    public bool CanEdit { get; set; }
    // The Head of the branch can change Add / Block of a flagged borrower.
    public bool CanChangeDecision { get; set; }
    // False until the survey is Approved: nothing to show yet.
    public bool IsOpen { get; set; }
    public List<BorrowerItem> Borrowers { get; set; } = new();
}

public class BorrowerDecisionRequest
{
    // Add or Block
    public string Decision { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
