namespace QubeFin.Persistence.Models.Hrms;

public class ApprovalWorkflowStep
{
    public Guid Id { get; private set; }
    public Guid ApprovalWorkflowId { get; private set; }
    public Guid ReceiverPostId { get; private set; }
    public Guid OrganizationUnitTypeId { get; private set; }
    public bool IsRecommendEvent { get; private set; }
    public bool IsApprovalEvent { get; private set; }
    public string EventStatus { get; private set; } = null!;
    public string EventButtonText { get; private set; } = null!;
    public int SequenceNo { get; private set; }

    public string? ReceiverPostName { get; private set; }
    public string? OrganizationUnitTypeName { get; private set; } = null!;
    private ApprovalWorkflowStep()
    {
    }

    public ApprovalWorkflowStep(
        Guid id,
        Guid approvalWorkflowId,
        Guid receiverPostId,
        Guid organizationUnitTypeId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo,
        string? organizationUnitTypeName = null,
        string? receiverPostName = null)
    {
        Id = id;
        ApprovalWorkflowId = approvalWorkflowId;
        OrganizationUnitTypeId = organizationUnitTypeId;
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
        SequenceNo = sequenceNo;
        OrganizationUnitTypeName = organizationUnitTypeName;
        ReceiverPostName = receiverPostName;
    }

    public static ApprovalWorkflowStep Create(
        Guid id,
        Guid approvalWorkflowId,
        Guid receiverPostId,
        Guid organizationUnitTypeId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo)
    {
        return new ApprovalWorkflowStep(id, approvalWorkflowId, receiverPostId, organizationUnitTypeId,isRecommendEvent, isApprovalEvent,
            eventStatus, eventButtonText, sequenceNo);
    }

    public void Update(
        Guid receiverPostId,
        Guid organizationUnitTypeId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo)
    {
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        OrganizationUnitTypeId = organizationUnitTypeId;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
        SequenceNo = sequenceNo;
    }
}
