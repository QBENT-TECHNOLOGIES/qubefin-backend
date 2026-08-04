using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblApprovalWorkflowEvent;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class ApprovalWorkflowEventMapper
{
    public static ApprovalWorkflowEvent ToDomain(this Entity entity)
    {
        return new ApprovalWorkflowEvent(
            entity.Id,
            entity.Category,
            entity.LeaveTypeId,
            entity.OrganizationUnitTypeId,
            entity.SalaryGradeId,
            entity.PostId,
            entity.MinimumDays,
            entity.MaximumDays,
            entity.SequenceNo,
            entity.ReceiverPostId,
            entity.IsRecommendEvent,
            entity.IsApprovalEvent,
            entity.EventStatus,
            entity.EventButtonText);
    }

    public static Entity ToEntity(this ApprovalWorkflowEvent domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Category = domain.Category,
            LeaveTypeId = domain.LeaveTypeId,
            OrganizationUnitTypeId = domain.OrganizationUnitTypeId,
            SalaryGradeId = domain.SalaryGradeId,
            PostId = domain.PostId,
            MinimumDays = domain.MinimumDays,
            MaximumDays = domain.MaximumDays,
            SequenceNo = domain.SequenceNo,
            ReceiverPostId = domain.ReceiverPostId,
            IsRecommendEvent = domain.IsRecommendEvent,
            IsApprovalEvent = domain.IsApprovalEvent,
            EventStatus = domain.EventStatus,
            EventButtonText = domain.EventButtonText
        };
    }
}
