namespace QubeFin.Persistence.Models.Collection;

public sealed class CreditOfficer
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? EmployeeCode { get; set; }
    public Guid DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public bool IsIncharge { get; set; }
    public Guid OrganizationUnitId { get; set; }
    public string OrganizationUnitName { get; set; } = string.Empty;
    public int OrganizationCode { get; set; }
    public string OrganizationUnitTypeName { get; set; } = string.Empty;
    public DateTime EffectiveFrom { get; set; }
}
