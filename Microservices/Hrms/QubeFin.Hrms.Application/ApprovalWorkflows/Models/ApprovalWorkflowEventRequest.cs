namespace QubeFin.Hrms.Application.ApprovalWorkflows.Models;

public record ApprovalWorkflowEventRequest(
    Guid? Id,
    string Category,
    Guid? LeaveTypeId,
    Guid? OrganizationUnitTypeId,
    Guid? SalaryGradeId,
    Guid? PostId,
    int MinimumDays,
    int? MaximumDays,
    int SequenceNo,
    Guid ReceiverPostId,
    bool IsRecommendEvent,
    bool IsApprovalEvent,
    string EventStatus,
    string EventButtonText);
