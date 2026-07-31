using Microsoft.AspNetCore.Http;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class RegularizationRequest
    {
        public Guid Id { get; set; }
        public string RegularizationType { get; set; } = string.Empty;
        public List<DateOnly> RegularizationDates { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public IFormFile? Attachment { get; set; }
    }
    public class RegularizationSubmit
    {
        public Guid Id { get; set; }
        public string Decision { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}
