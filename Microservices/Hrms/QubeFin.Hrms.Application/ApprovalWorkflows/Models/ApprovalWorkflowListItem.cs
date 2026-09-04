namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models
{
    public class ApprovalWorkflowListItem
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? OrganizationUnitTypeName { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? PostName { get; set; }
        public string? SalaryGradesName { get; set; }
        public int? MinimumDays { get; set; }
        public int? MaximumDays { get; set; }
        public string RangeDaysDisplay => (MinimumDays is null or 0 && MaximumDays is null or 0) ? "-" : $"{MinimumDays} - {MaximumDays}";
        public string ApprovalPath { get; set; } = string.Empty;
    }
}
