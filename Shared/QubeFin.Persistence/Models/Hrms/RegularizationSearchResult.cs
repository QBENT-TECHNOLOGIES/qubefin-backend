using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class RegularizationSearchResult
    {
        public Guid Id { get; set; }
        public string RegularizationType { get; set; } = string.Empty;
        public string RegularizationDate { get; set; } = string.Empty;
        public string Reason { get; set; } = null!;
        public string? Attachment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
        public int? TotalCount { get; set; }
    }
}
