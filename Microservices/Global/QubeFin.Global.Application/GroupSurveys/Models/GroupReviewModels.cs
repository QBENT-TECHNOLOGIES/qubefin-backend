using FluentValidation;
using QubeFin.Global.Application.SurveyApprovals.Models;

namespace QubeFin.Global.Application.GroupSurveys.Models
{
    public static class GroupRecommendationActions
    {
        public const string Recommend = "Recommend";
        public const string Recheck = "Recheck";
    }

    // What the Head of Branch sends: a recommendation, or a Recheck (back to the Credit Officers).
    public class GroupRecommendationRequest
    {
        public string Action { get; set; } = string.Empty;       // Recommend / Recheck
        public string? Recommendation { get; set; }                // Highly Recommended / Recommended / Not Recommended
        public string? Comments { get; set; }
    }

    public class GroupRecommendationRequestValidator : AbstractValidator<GroupRecommendationRequest>
    {
        public GroupRecommendationRequestValidator()
        {
            RuleFor(r => r.Action)
                .Must(a => a == GroupRecommendationActions.Recommend || a == GroupRecommendationActions.Recheck)
                .WithMessage("Action must be Recommend or Recheck.");

            // A recommendation needs the level; a Recheck needs the reason.
            RuleFor(r => r.Recommendation)
                .Must(v => !string.IsNullOrEmpty(v) && GroupSurveyOptions.Recommendation.Contains(v))
                .When(r => r.Action == GroupRecommendationActions.Recommend)
                .WithMessage($"Recommendation must be one of: {string.Join(", ", GroupSurveyOptions.Recommendation)}.");
            RuleFor(r => r.Comments).NotEmpty()
                .When(r => r.Action == GroupRecommendationActions.Recheck)
                .WithMessage("Comments are required for a Recheck.");
            RuleFor(r => r.Comments).MaximumLength(1000).WithMessage("Comments can have at most 1000 characters.");
        }
    }

    public class GroupRecommendationItem
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Recommendation { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateTime? ActionOn { get; set; }
    }

    // What the "Recommendation and Approval" card of a Group Survey needs.
    public class GroupReviewResponse
    {
        public Guid SurveyId { get; set; }
        public string Status { get; set; } = string.Empty;
        // The Head of Branch, while the survey is Pending Recommendation.
        public bool CanRecommend { get; set; }
        // The head of the unit above the branch, while the survey is Pending Approval.
        public bool CanApprove { get; set; }
        public GroupRecommendationItem? Recommendation { get; set; }
        // The Area Manager decision (Approve / Reject), shown to everyone who can see the survey.
        public ApprovalItem? Decision { get; set; }
        // After a Recheck: why the survey came back, and from whom.
        public string? RecheckComment { get; set; }
        public string? RecheckBy { get; set; }
    }
}
