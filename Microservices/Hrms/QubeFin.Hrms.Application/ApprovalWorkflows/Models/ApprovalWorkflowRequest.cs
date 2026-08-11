namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models;

public record ApprovalWorkflowRequest(
    string Category,
    Guid? LeaveTypeId,
    Guid? OrganizationUnitTypeId,
    Guid? SalaryGradeId,
    Guid? PostId,
    int MinimumDays,
    int MaximumDays,
    IReadOnlyList<ApprovalWorkflowStepRequest> Steps);

public record ApprovalWorkflowStepRequest(
    Guid? Id,
    Guid ReceiverPostId,
    bool IsRecommendEvent,
    bool IsApprovalEvent,
    string EventStatus,
    string EventButtonText,
    int SequenceNo);
