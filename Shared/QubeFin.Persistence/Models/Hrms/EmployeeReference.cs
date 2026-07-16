namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeReference
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string PersonName { get; private set; } = null!;
    public string Mobile { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? Occupation { get; private set; }
    public string? HowDoYouKnow { get; private set; }

    // Required for ORM/EF Core hydration
    private EmployeeReference()
    {
    }

    public EmployeeReference(
        Guid id,
        Guid employeeId,
        string personName,
        string mobile,
        string email,
        string address,
        string? occupation,
        string? howDoYouKnow)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        EmployeeId = employeeId;
        PersonName = personName.Trim();
        Mobile = mobile.Trim();
        Email = email.Trim();
        Address = address.Trim();
        Occupation = occupation?.Trim();
        HowDoYouKnow = howDoYouKnow?.Trim();
    }
}
