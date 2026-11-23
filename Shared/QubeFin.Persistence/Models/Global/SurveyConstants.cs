namespace QubeFin.Persistence.Models.Global
{
    // Survey types allowed in Tbl_Survey.SurveyType (kept in one place so the text is never typed twice).
    public static class SurveyTypes
    {
        public const string Branch = "Branch Survey";
        public const string Group = "Group Survey";
    }

    // Values of Tbl_Survey.Status. Only the statuses used so far are listed; more are added with each phase.
    public static class SurveyStatuses
    {
        public const string Assigned = "Assigned";
        public const string Draft = "Draft";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string BranchCreated = "BranchCreated";
    }
}
