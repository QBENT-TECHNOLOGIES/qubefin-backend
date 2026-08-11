namespace QubeFin.Persistence.Models.Hrms;

public class ApprovalWorkflow
{
    private readonly List<ApprovalWorkflowStep> _steps = [];

    public Guid Id { get; private set; }
    public string Category { get; private set; } = null!;
    public Guid? LeaveTypeId { get; private set; }
    public Guid? OrganizationUnitTypeId { get; private set; }
    public Guid? SalaryGradeId { get; private set; }
    public Guid? PostId { get; private set; }
    public int MinimumDays { get; private set; }
    public int MaximumDays { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public IReadOnlyCollection<ApprovalWorkflowStep> Steps => _steps;

    private ApprovalWorkflow()
    {
    }

    public ApprovalWorkflow(
        Guid id,
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int maximumDays,
        DateTime createdOn,
        Guid createdBy,
        DateTime? lastModifiedOn,
        Guid? lastModifiedBy,
        IEnumerable<ApprovalWorkflowStep>? steps = null)
    {
        Id = id;
        Category = category;
        LeaveTypeId = leaveTypeId;
        OrganizationUnitTypeId = organizationUnitTypeId;
        SalaryGradeId = salaryGradeId;
        PostId = postId;
        MinimumDays = minimumDays;
        MaximumDays = maximumDays;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;

        if (steps is not null)
        {
            _steps.AddRange(steps);
        }
    }

    public static ApprovalWorkflow Create(
        Guid id,
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int maximumDays,
        Guid createdBy,
        IEnumerable<ApprovalWorkflowStep>? steps = null)
    {
        return new ApprovalWorkflow(id, category, leaveTypeId, organizationUnitTypeId, salaryGradeId, postId,
            minimumDays, maximumDays, DateTime.Now, createdBy, null, null, steps);
    }

    public void Update(
        string category,
        Guid? leaveTypeId,
        Guid? organizationUnitTypeId,
        Guid? salaryGradeId,
        Guid? postId,
        int minimumDays,
        int maximumDays,
        Guid modifiedBy)
    {
        Category = category;
        LeaveTypeId = leaveTypeId;
        OrganizationUnitTypeId = organizationUnitTypeId;
        SalaryGradeId = salaryGradeId;
        PostId = postId;
        MinimumDays = minimumDays;
        MaximumDays = maximumDays;
        LastModifiedOn = DateTime.Now;
        LastModifiedBy = modifiedBy;
    }

    public void ReplaceSteps(IEnumerable<ApprovalWorkflowStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        _steps.Clear();
        _steps.AddRange(steps);
    }
}
