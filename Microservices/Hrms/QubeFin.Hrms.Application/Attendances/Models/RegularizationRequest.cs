using Microsoft.AspNetCore.Http;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class RegularizationRequest
    {
        public Guid Id { get; set; }
        public DateOnly RegularizationDate { get; set; }
        public string Reason { get; set; } = null!;
        public IFormFile? Attachment { get; set; }
    }
}
