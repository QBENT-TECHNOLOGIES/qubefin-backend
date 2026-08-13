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
        public string? Reason { get; set; } = null!;
        public string? Attachment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
        public int? TotalCount { get; set; }
    }
    public class RegularizationResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? RegularizationType { get; set; }
        public string? RegularizationDates { get; set; }
        public string? Reason { get; set; }
        public string? Attachment { get; set; }
        public string? Remarks { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CurrentStatus { get; set; }
        public bool IsRecommendEvent { get; set; }
        public bool IsApprovalEvent { get; set; }
        public string? ApprovalCategory { get; set; }
        public DateTime? EventDate { get; set; }
        public string? EventRemarks { get; set; }
        public string? SenderDesignation { get; set; }
        public string? ReceiverDesignation { get; set; }
        public string? EventCategory { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
    }
    public class RegularizationApprovalSearchResult
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string OrganizationUnit { get; set; } = string.Empty;
        public string RegularizationType { get; set; } = string.Empty;
        public string RegularizationDate { get; set; } = string.Empty;
        public string? Reason { get; set; } = null!;
        public string? Attachment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? Status { get; set; }
        public string? Category { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
        public int? TotalCount { get; set; }
    }
}
