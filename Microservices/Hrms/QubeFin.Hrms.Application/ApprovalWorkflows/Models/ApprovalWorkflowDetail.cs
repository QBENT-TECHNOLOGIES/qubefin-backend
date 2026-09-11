using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models
{
    public class ApprovalWorkflowDetail
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public Guid? OrganizationUnitTypeId { get; set; }
        public Guid? LeaveTypeId { get; set; }
        public Guid? PostId { get; set; }
        public List<Guid>? SalaryGradeIds { get; set; }
        public int? MinimumDays { get; set; }
        public int? MaximumDays { get; set; }
        public string? LeaveTypeName { get; set; }
        public string? SalaryGradesName { get; set; }
        public string? OrganizationUnitTypeName { get; set; }
        public string? PostName { get; set; }
        public string? CreatedByName { get; set; }
        public string? LastModifiedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ApprovalWorkflowStep>? Steps { get; set; }
        public string? StepPost { get; set; }
        public List<ApprovalStep> ApprovalSteps { get; set; } = new();
    }

    public class ApprovalStep
    {
        public Guid Id { get; set; }
        public Guid ApprovalWorkflowId { get; set; }
        public Guid? OrganizationUnitTypeId { get; set; }
        public Guid ReceiverPostId { get; set; }
        public bool IsRecommendEvent { get; set; }
        public bool IsApprovalEvent { get; set; }
        public string EventStatus { get; set; } = string.Empty;
        public string EventButtonText { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
        public string? OrganizationUnitTypeName { get; set; }
    }
}
