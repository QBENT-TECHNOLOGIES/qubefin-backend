namespace QubeFin.Persistence.Models.Hrms
{
    public class LeavePrayer
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid LeaveTypeId { get; set; }
        public int PrayerDays { get; set; }
        public string? Attachment { get; set; }
        public string? Remarks { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CurrentStatus { get; set; } = null!;
        private LeavePrayer() { }
        public LeavePrayer(Guid id,Guid employeeId, Guid leaveTypeId, int prayerDays, string? attachment, string? remarks, Guid createdBy, DateTime createdOn, string currentStatus)
        {
            Id = id;
            EmployeeId = employeeId;
            LeaveTypeId = leaveTypeId;
            PrayerDays = prayerDays;
            Attachment = attachment;
            Remarks = remarks;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            CurrentStatus = currentStatus;
        }
    }

    public class LeavePrayerResponse
    {
        public Guid? Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? LeaveType { get; set; }
        public Guid? LeaveTypeId { get; set; }
        public int? PrayerDays { get; set; }
        public string? CurrentStatus { get; set; }
        public string? LeavePrayerRemarks { get; set; }
        public string? Attachment { get; set; }
        public DateTime? AppliedOn { get; set; }
        public string? ApprovalCategory { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Remarks { get; set; }
        public string? SenderDesignation { get; set; }
        public string? ReceiverDesignation { get; set; }
        public string? EventCategory { get; set; }
        public string? EventStatus { get; set; }
        public string? EventButtonText { get; set; }
        public bool? IsRecommendEvent { get; set; }
        public bool? IsApprovalEvent { get; set; }
    }
}
