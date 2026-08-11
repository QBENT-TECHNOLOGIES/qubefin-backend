using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblApprovalWorkflowStep;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class ApprovalWorkflowStepMapper
{
    public static ApprovalWorkflowStep ToDomain(this Entity entity)
    {
        return new ApprovalWorkflowStep(
            entity.Id,
            entity.ApprovalWorkflowId,
            entity.ReceiverPostId,
            entity.IsRecommendEvent,
            entity.IsApprovalEvent,
            entity.EventStatus,
            entity.EventButtonText,
            entity.SequenceNo);
    }

    public static Entity ToEntity(this ApprovalWorkflowStep domain)
    {
        return new Entity
        {
            Id = domain.Id,
            ApprovalWorkflowId = domain.ApprovalWorkflowId,
            ReceiverPostId = domain.ReceiverPostId,
            IsRecommendEvent = domain.IsRecommendEvent,
            IsApprovalEvent = domain.IsApprovalEvent,
            EventStatus = domain.EventStatus,
            EventButtonText = domain.EventButtonText,
            SequenceNo = domain.SequenceNo
        };
    }
}
