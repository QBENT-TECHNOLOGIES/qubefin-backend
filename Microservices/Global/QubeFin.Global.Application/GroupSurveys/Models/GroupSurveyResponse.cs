namespace QubeFin.Global.Application.GroupSurveys.Models
{
    public class GroupSurveyResponse
    {
        public Guid SurveyId { get; set; }
        public string Status { get; set; } = string.Empty;
        // True only for the Lead Credit Officer while the survey is still Assigned or Draft.
        public bool CanEditForm { get; set; }
        // False until the Lead Credit Officer saves the form for the first time.
        public bool IsSaved { get; set; }
        public DateTime? LastSavedOn { get; set; }
        public GroupSurveyForm Form { get; set; } = new();
        // "Surveyed By" block of the declaration: filled once the form is submitted.
        public SignOffResponse? SurveyedBy { get; set; }
        // "Recommended By" block: filled when the Head of Branch has recommended the survey.
        public SignOffResponse? RecommendedBy { get; set; }
        // "Approved By" block: filled when the Area Manager has approved the survey.
        public SignOffResponse? ApprovedBy { get; set; }
    }

    // One signing block: name, designation, PIN (employee code), date, and a short-lived link to the signature image.
    public class SignOffResponse
    {
        public string Name { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? Pin { get; set; }
        public DateTime? Date { get; set; }
        public string? SignatureUrl { get; set; }
    }
}
