namespace QubeFin.Global.Application.BranchSurveys.Models
{
    public class BranchSurveyResponse
    {
        public Guid SurveyId { get; set; }
        public string Status { get; set; } = string.Empty;
        // True only for the Lead Surveyor while the survey is still Assigned or Draft.
        public bool CanEditForm { get; set; }
        // False until the Lead Surveyor saves the form for the first time.
        public bool IsSaved { get; set; }
        public DateTime? LastSavedOn { get; set; }
        public BranchSurveyForm Form { get; set; } = new();
    }
}
