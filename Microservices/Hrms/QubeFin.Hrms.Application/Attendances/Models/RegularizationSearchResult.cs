using System;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class RegularizationSearchResult
    {
        public Guid Id { get; set; }
        public DateOnly RegularizationDate { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime AppliedOn { get; set; }
        public string? Status { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}
