namespace QubeFin.Hrms.Application.Leaves.Models
{
    public class LeaveRequestDetailResponse
    {
        public Guid? Id { get; set; }
        public string? LeaveType { get; set; }
        public Guid? LeaveTypeId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? TotalDays { get; set; }
        public string? CurrentStatus { get; set; }
        public string? Reason { get; set; }
        public string? Address { get; set; }
        public string? EnclosedDocName { get; set; }
        public string? EnclosedDocNo { get; set; }
        public bool? IsSubmitted { get; set; }
        public bool? IsCancellable { get; set; }
        public string? RejectedReason { get; set; }
        public List<LeaveRequestEvent> Events { get; set;  } = new List<LeaveRequestEvent>();

    }

    public class LeaveRequestEvent
    {
        public string? Event { get; set;  }
        public string? EventBy { get; set; }
        public DateTime? EventOn { get; set; }
        public string? SenderDesignation { get; set; }
        public string? ReceiverDesignation { get; set; }
        public string? EventRemarks { get; set; }
    }
}
