namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models;

public record ApprovalWorkflowRequest(
    string Category,
    Guid? LeaveTypeId,
    Guid? OrganizationUnitTypeId, 
    IReadOnlyList<Guid>? SalaryGradeIds,
    Guid? PostId,
    int MinimumDays,
    int MaximumDays,
    IReadOnlyList<ApprovalWorkflowStepRequest> Steps);

public record ApprovalWorkflowStepRequest(
    Guid? Id,
    Guid ReceiverPostId,
    Guid OrganizationUnitTypeId,
    bool IsRecommendEvent,
    bool IsApprovalEvent,
    string EventStatus,
    string EventButtonText,
    int SequenceNo);
