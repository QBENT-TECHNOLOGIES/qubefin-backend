namespace QubeFin.Global.Application.BranchSurveys.Models
{
    // The choices allowed in the dropdown fields of the Branch Survey form.
    public static class BranchSurveyOptions
    {
        public static readonly string[] Rating = ["Excellent", "Good", "Average", "Poor"];
        public static readonly string[] AdministrativeStatus = ["Rural", "Semi Urban", "Urban"];
        public static readonly string[] EconomicCondition = ["Good", "Average", "Poor"];
        public static readonly string[] NaturalRisk = ["High", "Medium", "Low"];
        public static readonly string[] RiskLevel = ["Low", "Medium", "High"];
        public static readonly string[] YesNo = ["Yes", "No"];
        public static readonly string[] Recommendation = ["Highly Recommended", "Recommended", "Not Recommended"];
    }
}
