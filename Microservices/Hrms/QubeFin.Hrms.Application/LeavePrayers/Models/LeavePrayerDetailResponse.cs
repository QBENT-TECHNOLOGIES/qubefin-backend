using QubeFin.Hrms.Application.Leaves.Models;

namespace QubeFin.Hrms.Application.LeavePrayers.Models
{
    public class LeavePrayerDetailResponse
    {
        public Guid? Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? LeaveType { get; set; }
        public Guid? LeaveTypeId { get; set; }
        public int? PrayerDays { get; set; }
        public string? CurrentStatus { get; set; }
         public string? LeavePrayerRemarks { get; set; }
        public string? Attachment { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime? AppliedOn { get; set; }
        public string? ApprovalCategory { get; set; }
        public string? EventButtonText { get; set; }
        public bool? IsRecommendEvent { get; set; }
        public bool? IsApprovalEvent { get; set; }
        public List<LeaveRequestEvent> Events { get; set; } = new List<LeaveRequestEvent>();
    }
}
