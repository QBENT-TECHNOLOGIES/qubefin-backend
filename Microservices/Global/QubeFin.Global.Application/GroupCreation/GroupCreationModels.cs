using FluentValidation;
using QubeFin.Global.Application.Groups.Models;

namespace QubeFin.Global.Application.GroupCreation;

public static class GroupRoles
{
    public const string President = "President";
    public const string Secretary = "Secretary";
    public const string Cashier = "Cashier";
    public static readonly string[] All = [President, Secretary, Cashier];
}

// One eligible borrower of the group, as the Credit Officer fills it in.
public class GroupMemberRequest
{
    public Guid BorrowerId { get; set; }
    public decimal ProposedLoanAmount { get; set; }
    public string LoanPurpose { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    // President, Secretary or Cashier; null for a plain member.
    public string? Role { get; set; }
    // Needed when the member is a leader.
    public DateOnly? AppointmentDate { get; set; }
}

// What the Credit Officer saves in the Group Creation window (after a GRT Pass).
public class CreateGroupFromSurveyRequest
{
    public string Name { get; set; } = string.Empty;
    public DateOnly FormationDate { get; set; }
    public int CollectionDay { get; set; }
    public TimeOnly CollectionTime { get; set; }
    public string MobileNo { get; set; } = string.Empty;
    public string SittingPlace { get; set; } = string.Empty;
    public string? PoliceStation { get; set; }
    public string? PostOffice { get; set; }
    public string? PinCode { get; set; }
    // Road
    public string? Address { get; set; }
    public string? LandMark { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public List<GroupMemberRequest> Members { get; set; } = new();
}

public class CreateGroupFromSurveyRequestValidator : AbstractValidator<CreateGroupFromSurveyRequest>
{
    public CreateGroupFromSurveyRequestValidator()
    {
        RuleFor(v => v.Name).NotEmpty().WithMessage("Group Name is required.").MaximumLength(50).WithMessage("Group Name can be 50 characters at most.");
        RuleFor(v => v.FormationDate).NotEqual(default(DateOnly)).WithMessage("Date of Group Formation is required.");
        RuleFor(v => v.CollectionDay).InclusiveBetween(1, 7).WithMessage("Group Day is required.");
        RuleFor(v => v.MobileNo).NotEmpty().WithMessage("Mobile No is required.").Matches(@"^\d{10}$").WithMessage("Mobile No must be 10 digits.");
        RuleFor(v => v.SittingPlace).NotEmpty().WithMessage("Sitting Place is required.").MaximumLength(200).WithMessage("Sitting Place can be 200 characters at most.");
        RuleFor(v => v.PoliceStation).MaximumLength(100).WithMessage("Police Station can be 100 characters at most.");
        RuleFor(v => v.PostOffice).MaximumLength(100).WithMessage("Post Office can be 100 characters at most.");
        RuleFor(v => v.PinCode).Matches(@"^\d{6}$").When(v => !string.IsNullOrWhiteSpace(v.PinCode)).WithMessage("PIN Code must be 6 digits.");
        RuleFor(v => v.Address).MaximumLength(500).WithMessage("Address can be 500 characters at most.");
        RuleFor(v => v.LandMark).MaximumLength(100).WithMessage("Landmark can be 100 characters at most.");
        RuleFor(v => v.Latitude).InclusiveBetween(-90m, 90m).When(v => v.Latitude.HasValue).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(v => v.Longitude).InclusiveBetween(-180m, 180m).When(v => v.Longitude.HasValue).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(v => v).Must(v => v.Latitude.HasValue == v.Longitude.HasValue).WithMessage("Enter both Latitude and Longitude, or neither.");

        RuleFor(v => v.Members).NotEmpty().WithMessage("Select at least one eligible borrower.");
        RuleForEach(v => v.Members).ChildRules(m =>
        {
            m.RuleFor(x => x.BorrowerId).NotEmpty().WithMessage("Borrower is required.");
            m.RuleFor(x => x.ProposedLoanAmount).InclusiveBetween(0m, 999999999999m).WithMessage("Proposed Loan Amount is not valid.");
            m.RuleFor(x => x.LoanPurpose).NotEmpty().WithMessage("Purpose of Loan is required for every member.").MaximumLength(200).WithMessage("Purpose of Loan can be 200 characters at most.");
            m.RuleFor(x => x.Remarks).MaximumLength(500).WithMessage("Remarks can be 500 characters at most.");
            m.RuleFor(x => x.Role).Must(r => r is null || GroupRoles.All.Contains(r)).WithMessage("Role must be President, Secretary or Cashier.");
            m.RuleFor(x => x.AppointmentDate).NotNull().When(x => x.Role != null).WithMessage("Date of Appointment is required for a leader.");
        });
        RuleFor(v => v.Members).Must(m => m.Select(x => x.BorrowerId).Distinct().Count() == m.Count)
            .WithMessage("A borrower is selected twice.");
        // Exactly one President, one Secretary and one Cashier.
        RuleFor(v => v.Members).Must(m => GroupRoles.All.All(role => m.Count(x => x.Role == role) == 1))
            .When(v => v.Members.Count > 0).WithMessage("Choose exactly one President, one Secretary and one Cashier from the selected members.");
    }
}

public class CreateInfoBorrower
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string GuardianName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public int Age { get; set; }
    public string MaritalStatus { get; set; } = string.Empty;
}

public class CreateInfoPerson
{
    public string Name { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Code { get; set; }
}

// What the Group Creation window needs.
public class GroupCreateInfoResponse
{
    public Guid SurveyId { get; set; }
    // The survey's Credit Officer can create it: GRT submitted as Pass, no group yet.
    public bool CanCreate { get; set; }
    // Why it is not possible now (shown on the card).
    public string? Reason { get; set; }
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? GroupCode { get; set; }

    public string? BranchName { get; set; }
    public string ProposedArea { get; set; } = string.Empty;
    public Guid AdministrativeUnitId { get; set; }
    public string? AreaName { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? LandMark { get; set; }

    // CGT and GRT are both Complete once the GRT is a Pass.
    public string CgtStatus { get; set; } = "Pending";
    public string GrtStatus { get; set; } = "Pending";
    public CreateInfoPerson? ConductedBy { get; set; }

    // The Responsible Credit Officer: the person who creates the group.
    public CreateInfoPerson? CreditOfficer { get; set; }
    public DateOnly? CreditOfficerJoinedOn { get; set; }

    public List<CreateInfoBorrower> Borrowers { get; set; } = new();
}
