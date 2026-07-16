namespace QubeFin.Hrms.Api.Requests;

public class OfficialInfoRequest
{
    public Guid? CompanyId { get;  set; }
    public Guid? OrganizationUnitId { get;  set; }
    public Guid? DepartmentId { get;  set; }
    public string? EmployementType { get;  set; }
    public DateTime? DateOfJoining { get;  set; }
    public DateTime? DateOfConfirmation { get;  set; }
    public DateTime? SeparationDate { get;  set; }
    public Guid? ReferedBy { get;  set; }
    public string? HowYouKnow { get;  set; }
    public string? OfficialEmail { get;  set; }
    public bool IsActive { get;  set; }
}
