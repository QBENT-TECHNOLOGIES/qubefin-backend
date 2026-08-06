using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class RegularizationDetailResponse
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
        public List<RegularizationEvent> Events { get; set; } = [];
    }
    public class RegularizationEvent
    {
        public string? ApprovalCategory { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Remarks { get; set; }
        public string? SenderDesignation { get; set; }
        public string? ReceiverDesignation { get; set; }
        public string? EventCategory { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
    }
}
