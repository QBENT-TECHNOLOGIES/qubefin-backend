namespace QubeFin.Persistence.Models.Hrms;

public class ApprovalWorkflowEventGroupItem
{
    public string Category { get; set; } = string.Empty;
    public Guid OrganizationUnitTypeId { get; set; }
    public string OrganizationUnitType { get; set; } = string.Empty;
    public Guid? LeaveTypeId { get; set; }
    public string? LeaveType { get; set; }
    public Guid? SalaryGradeId { get; set; }
    public string? SalaryGrade { get; set; }
    public int MinimumDays { get; set; }
    public int MaximumDays { get; set; }
    public string RangeDays { get; set; } = string.Empty;
    public string WorkflowEventPath { get; set; } = string.Empty;
}
