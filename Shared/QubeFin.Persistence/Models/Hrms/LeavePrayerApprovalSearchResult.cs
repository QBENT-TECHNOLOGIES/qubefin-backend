namespace QubeFin.Persistence.Models.Hrms
{
    public class LeavePrayerApprovalSearchResult
    {
        public Guid? Id { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? LeaveType { get; set; }
        public DateOnly? AppliedOn { get; set; }
        public string? Status { get; set; }
        public int? PrayerDays { get; set; }
        public int? TotalCount { get; set; }        
    }
}
