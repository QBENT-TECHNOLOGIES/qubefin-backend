using QubeFin.Persistence.Models.Global;

namespace QubeFin.Hrms.Application.Departments.Models;

public class DepartmentResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid? HodEmployeeId { get; set; }

    public string? HodEmployeeName { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public AuditInfo? AuditInfo { get; set; } = new AuditInfo();
}