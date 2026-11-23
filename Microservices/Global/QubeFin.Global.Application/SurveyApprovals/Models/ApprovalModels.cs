namespace QubeFin.Global.Application.SurveyApprovals.Models
{
    // The MD sends the same Decision + Comments as the committee (EvaluationRequest).
    public class ApprovalItem
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateTime ActionOn { get; set; }
    }

    public class SurveyApprovalResponse
    {
        public Guid SurveyId { get; set; }
        public string Status { get; set; } = string.Empty;
        // The Managing Director, while the survey waits for approval.
        public bool CanDecide { get; set; }
        // The MD's decision. Shown to everyone who can see the survey (not after a Recheck, which starts the process again).
        public ApprovalItem? Decision { get; set; }
    }
}
