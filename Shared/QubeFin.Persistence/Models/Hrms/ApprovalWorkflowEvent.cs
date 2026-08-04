namespace QubeFin.Persistence.Models.Hrms;

public class ApprovalWorkflowEvent
{
    public Guid Id { get; private set; }
    public string Category { get; private set; } = null!;
    public Guid? LeaveTypeId { get; private set; }
    public Guid? OrganizationUnitTypeId { get; private set; }
    public Guid? SalaryGradeId { get; private set; }
    public Guid? PostId { get; private set; }
    public int MinimumDays { get; private set; }
    public int? MaximumDays { get; private set; }
    public int SequenceNo { get; private set; }
    public Guid ReceiverPostId { get; private set; }
    public bool IsRecommendEvent { get; private set; }
    public bool IsApprovalEvent { get; private set; }
    public string EventStatus { get; private set; } = null!;
    public string EventButtonText { get; private set; } = null!;

    private ApprovalWorkflowEvent()
    {
    }

    public ApprovalWorkflowEvent(
        Guid id,
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int? maximumDays,
        int sequenceNo,
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText)
    {
        Id = id;
        Category = category;
        LeaveTypeId = leaveTypeId;
        OrganizationUnitTypeId = organizationUnitTypeId;
        SalaryGradeId = salaryGradeId;
        PostId = postId;
        MinimumDays = minimumDays;
        MaximumDays = maximumDays;
        SequenceNo = sequenceNo;
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
    }

    public static ApprovalWorkflowEvent Create(
        Guid id,
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int? maximumDays,
        int sequenceNo,
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText)
    {
        return new ApprovalWorkflowEvent(id, category, leaveTypeId, organizationUnitTypeId, salaryGradeId, postId,
            minimumDays, maximumDays, sequenceNo, receiverPostId, isRecommendEvent, isApprovalEvent, eventStatus, eventButtonText);
    }

    public void Update(
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int? maximumDays,
        int sequenceNo,
        Guid receiverPostId,
        bool isRecommendEvent,
        bool isApprovalEvent,
        string eventStatus,
        string eventButtonText)
    {
        Category = category;
        LeaveTypeId = leaveTypeId;
        OrganizationUnitTypeId = organizationUnitTypeId;
        SalaryGradeId = salaryGradeId;
        PostId = postId;
        MinimumDays = minimumDays;
        MaximumDays = maximumDays;
        SequenceNo = sequenceNo;
        ReceiverPostId = receiverPostId;
        IsRecommendEvent = isRecommendEvent;
        IsApprovalEvent = isApprovalEvent;
        EventStatus = eventStatus;
        EventButtonText = eventButtonText;
    }
}
