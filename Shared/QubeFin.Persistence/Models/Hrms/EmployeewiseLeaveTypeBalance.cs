namespace QubeFin.Persistence.Models.Hrms;

public class EmployeewiseLeaveTypeBalance
{
    public Guid LeaveTypeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public decimal LeaveEntitled { get; set; }
    public decimal LeaveTaken { get; set; }
    public decimal LeaveBalance { get; set; }
}
