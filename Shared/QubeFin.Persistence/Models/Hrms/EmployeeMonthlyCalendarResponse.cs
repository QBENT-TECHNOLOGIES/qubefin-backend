namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeMonthlyCalendarResponse
    {
        public DateOnly? CalendarDate { get; set; }
        public string? DayName { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public TimeOnly? ActualInTime { get; set; }
        public TimeOnly? ActualOutTime { get; set; }
    }
    public class EmployeeLeaveMonthlyCalendarResponse
    {
        public DateOnly? CalendarDate { get; set; }
        public string? DayName { get; set; }
        public string? Status { get; set; }
        public string? LeaveType { get; set; }

    }
}
