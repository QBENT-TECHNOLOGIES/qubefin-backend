using System;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class RegularizationSearchResult
    {
        public Guid Id { get; set; }
        public string RegularizationDate { get; set; } = string.Empty;
        public string Reason { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
        public string? AttachmentUrl { get; set; }
        public int? TotalCount { get; set; }
    }
}
