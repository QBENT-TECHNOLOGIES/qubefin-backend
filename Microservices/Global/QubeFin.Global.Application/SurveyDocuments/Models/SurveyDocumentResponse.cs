namespace QubeFin.Global.Application.SurveyDocuments.Models
{
    public class SurveyDocumentResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        // A short-lived link (about 30 minutes) to open the photo.
        public string Url { get; set; } = string.Empty;
        public DateTime? CreatedOn { get; set; }
    }
}
