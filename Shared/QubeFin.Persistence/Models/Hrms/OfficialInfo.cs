namespace QubeFin.Persistence.Models.Hrms;

public class OfficialInfo
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

    public OfficialInfo()
    {
    }

    public OfficialInfo(Guid? companyId, Guid? organizationUnitId, Guid? departmentId, string? employementType, DateOnly? dateOfJoining, DateOnly? dateOfConfirmation,
        DateOnly? separationDate, Guid? referedBy, string? howYouKnow, string? officialEmail, bool isActive)
    {
        CompanyId = companyId;
        OrganizationUnitId = organizationUnitId;
        DepartmentId = departmentId;
        EmployementType = employementType;
        DateOfJoining = dateOfJoining;
        DateOfConfirmation = dateOfConfirmation;
        SeparationDate = separationDate;
        ReferedBy = referedBy;
        HowYouKnow = howYouKnow;
        OfficialEmail = officialEmail;
        IsActive = isActive;
    }
}
