using FluentValidation;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyEvaluations.Models
{
    // What a committee member (or the Lead, for the final decision) sends.
    public class EvaluationRequest
    {
        public string Decision { get; set; } = string.Empty;   // Approve / Reject / Recheck
        public string? Comments { get; set; }
    }

    // Comments are required for Reject and Recheck. For Approve they are optional.
    public class EvaluationRequestValidator : AbstractValidator<EvaluationRequest>
    {
        public EvaluationRequestValidator()
        {
            RuleFor(r => r.Decision)
                .Must(d => d == EvaluationDecisions.Approve || d == EvaluationDecisions.Reject || d == EvaluationDecisions.Recheck)
                .WithMessage("Decision must be Approve, Reject or Recheck.");

            RuleFor(r => r.Comments).NotEmpty()
                .When(r => r.Decision == EvaluationDecisions.Reject || r.Decision == EvaluationDecisions.Recheck)
                .WithMessage("Comments are required for Reject and Recheck.");

            RuleFor(r => r.Comments).MaximumLength(500).WithMessage("Comments can have at most 500 characters.");
        }
    }

    public class EvaluationItem
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateTime EvaluationDate { get; set; }
        public bool IsFinal { get; set; }
    }

    public class SurveyEvaluationsResponse
    {
        public Guid SurveyId { get; set; }
        public string Status { get; set; } = string.Empty;
        // A committee member (not a Lead) who can still save or change an evaluation.
        public bool CanEvaluate { get; set; }
        // The Committee Lead, once every required member has evaluated.
        public bool CanGiveFinal { get; set; }
        public bool IsCommitteeLead { get; set; }
        public int RequiredCount { get; set; }
        public int DoneCount { get; set; }
        // Names of the members who have not evaluated yet (only filled for the Committee Lead).
        public List<string> PendingMembers { get; set; } = new();
        public EvaluationItem? MyEvaluation { get; set; }
        // The Lead sees every member's evaluation. A member sees only his own.
        public List<EvaluationItem> Evaluations { get; set; } = new();
        // The Lead's final decision, visible to everyone who can see the survey.
        public EvaluationItem? FinalEvaluation { get; set; }
        // After a Recheck, the Lead's comment for the surveyors.
        public string? RecheckComment { get; set; }
        // Who asked for the recheck: "Committee Lead" or "Managing Director".
        public string? RecheckBy { get; set; }
    }
}
