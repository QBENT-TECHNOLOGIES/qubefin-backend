namespace QubeFin.Persistence.Models.Hrms;

using System.Text.Json.Serialization;

public class ApprovalWorkflow
{
    private readonly List<ApprovalWorkflowStep> _steps = [];

    public Guid Id { get; private set; }
    public string Category { get; private set; } = null!;
    public Guid? LeaveTypeId { get; private set; }
    public Guid? OrganizationUnitTypeId { get; private set; }
    public Guid? SalaryGradeId { get; private set; }
    public Guid? PostId { get; private set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LeaveTypeName { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SalaryGradeName { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OrganizationUnitTypeName { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PostName { get; private set; }
    public int MinimumDays { get; private set; }
    public int MaximumDays { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string CreatedByName { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastModifiedByName { get; private set; }
    public IReadOnlyCollection<ApprovalWorkflowStep> Steps => _steps;
    public string StepPost => string.Join(" -> ", _steps
        .OrderBy(step => step.SequenceNo)
        .Select(step => step.ReceiverPostName)
        .Where(name => !string.IsNullOrWhiteSpace(name)));

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
        string? createdByName,
        DateTime? lastModifiedOn,
        Guid? lastModifiedBy,
        string? lastModifiedByName,
        IEnumerable<ApprovalWorkflowStep>? steps = null,
        string? leaveTypeName = null,
        string? salaryGradeName = null,
        string? organizationUnitTypeName = null,
        string? postName = null)
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
        CreatedByName = createdByName;

        LastModifiedOn = lastModifiedOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedByName = lastModifiedByName;
        LeaveTypeName = leaveTypeName;
        SalaryGradeName = salaryGradeName;
        OrganizationUnitTypeName = organizationUnitTypeName;
        PostName = postName;

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
            minimumDays, maximumDays, DateTime.Now, createdBy, null, null, null, null, steps);
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
