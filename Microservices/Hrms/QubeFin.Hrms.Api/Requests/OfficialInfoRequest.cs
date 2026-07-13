namespace QubeFin.Hrms.Api.Requests;

public class OfficialInfoRequest
{
    public Guid? CompanyId { get; private set; }
    public Guid? OrganizationUnitId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public string? EmployementType { get; private set; }
    public DateOnly? DateOfJoining { get; private set; }
    public DateOnly? DateOfConfirmation { get; private set; }
    public DateOnly? SeparationDate { get; private set; }
    public Guid? ReferedBy { get; private set; }
    public string? HowYouKnow { get; private set; }
    public string? OfficialEmail { get; private set; }
    public bool IsActive { get; private set; }
}
