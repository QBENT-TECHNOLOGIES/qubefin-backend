namespace QubeFin.Persistence.Models.Hrms;

public class ApprovalWorkflowStep
{
    public Guid Id { get; private set; }
    public Guid ApprovalWorkflowId { get; private set; }
    public Guid ReceiverPostId { get; private set; }
    public bool IsRecommendEvent { get; private set; }
    public bool IsApprovalEvent { get; private set; }
    public string EventStatus { get; private set; } = null!;
    public string EventButtonText { get; private set; } = null!;
    public int SequenceNo { get; private set; }

    private ApprovalWorkflowStep()
    {
    }

    public ApprovalWorkflowStep(
        Guid id,
        Guid approvalWorkflowId,
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo)
    {
        Id = id;
        ApprovalWorkflowId = approvalWorkflowId;
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
        SequenceNo = sequenceNo;
    }

    public static ApprovalWorkflowStep Create(
        Guid id,
        Guid approvalWorkflowId,
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo)
    {
        return new ApprovalWorkflowStep(id, approvalWorkflowId, receiverPostId, isRecommendEvent, isApprovalEvent,
            eventStatus, eventButtonText, sequenceNo);
    }

    public void Update(
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText,
        int sequenceNo)
    {
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
        SequenceNo = sequenceNo;
    }
}
