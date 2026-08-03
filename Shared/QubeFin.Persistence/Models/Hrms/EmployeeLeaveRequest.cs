namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeLeaveRequest
{
    public Guid Id { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDays { get; set; }
    public DateTime RequestDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public bool IsSubmitted { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
}
