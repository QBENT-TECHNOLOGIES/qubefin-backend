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
}
