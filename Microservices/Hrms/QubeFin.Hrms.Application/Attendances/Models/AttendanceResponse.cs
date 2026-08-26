namespace QubeFin.Hrms.Application.Attendances.Models
{
    public class AttendanceResponse
    {
        public Guid? Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? OrganizationUnitId { get; set; }
        public DateOnly? AttendanceDate { get; set; }
        public TimeOnly? ExpectedInTime { get; set; }
        public TimeOnly? ExpectedOutTime { get; set; }
        public TimeOnly? ActualInTime { get; set; }
        public TimeOnly? ActualOutTime { get; set; }
        public bool? IsEarlyLeave { get; set; }
        public bool? IsLateEntry { get; set; }
        public bool? IsFitnessReportRequired { get; set; }
        public bool? IsFitnessReportUploaded { get; set; }
        public decimal? InTimeLatitude { get; set; }
        public decimal? InTimeLongitude { get; set; }
        public decimal? OutTimeLatitude { get; set; }
        public decimal? OutTimeLongitude { get; set; }
    }
}
