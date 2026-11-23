namespace QubeFin.Persistence.Models.Global
{
    // Survey types allowed in Tbl_Survey.SurveyType (kept in one place so the text is never typed twice).
    public static class SurveyTypes
    {
        public const string Branch = "Branch Survey";
        public const string Group = "Group Survey";
    }

    // Values of Tbl_SurveyCommitteeEvaluation.Status (a committee member's or the Lead's decision).
    // Tbl_SurveyApproval.Action allows only these values (check constraint CK_Tbl_SurveyApproval_Action).
    public static class ApprovalActions
    {
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Recheck = "Recheck";

        // The screen and the API use Approve / Reject / Recheck (EvaluationDecisions).
        public static string FromDecision(string decision) => decision switch
        {
            EvaluationDecisions.Approve => Approved,
            EvaluationDecisions.Reject => Rejected,
            _ => Recheck
        };

        public static string ToDecision(string action) => action switch
        {
            Approved => EvaluationDecisions.Approve,
            Rejected => EvaluationDecisions.Reject,
            _ => EvaluationDecisions.Recheck
        };
    }

    public static class EvaluationDecisions
    {
        public const string Approve = "Approve";
        public const string Reject = "Reject";
        public const string Recheck = "Recheck";
    }

    // Values of Tbl_Survey.Status. Only the statuses used so far are listed; more are added with each phase.
    public static class SurveyStatuses
    {
        public const string Assigned = "Assigned";
        public const string Draft = "Draft";
        public const string PendingEvaluation = "PendingEvaluation";
        public const string EvaluationInProgress = "EvaluationInProgress";
        public const string PendingLeadEvaluation = "PendingLeadEvaluation";
        public const string PendingApproval = "PendingApproval";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string BranchCreated = "BranchCreated";
    }
}
