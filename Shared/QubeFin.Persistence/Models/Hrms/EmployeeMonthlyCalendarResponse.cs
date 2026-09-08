namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeMonthlyCalendarResponse
    {
        public DateOnly? CalendarDate { get; set; }
        public string? DayName { get; set; }
        public string? Status { get; set; }

    }
}
