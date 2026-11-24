namespace QubeFin.Global.Application.GroupSurveys.Models
{
    // The choices allowed in the dropdown fields of the Group Survey form.
    public static class GroupSurveyOptions
    {
        public static readonly string[] MainOccupation = ["Agriculture", "Agricultural Labour", "Dairy", "Livestock", "Small Business", "Small Industries", "Transport", "Service", "Daily Labour", "Other"];
        public static readonly string[] YesNo = ["Yes", "No"];
        public static readonly string[] GoodModeratePoor = ["Good", "Moderate", "Poor"];
        public static readonly string[] StableModerateSensitive = ["Stable", "Moderate", "Sensitive"];
        public static readonly string[] Recommendation = ["Highly Recommended", "Recommended", "Not Recommended"];
    }
}
