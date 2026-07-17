namespace QubeFin.Hrms.Application.Employees.Models;

public class ReferenceDetailRequest
{
    public Guid Id { get;  set; }
    public Guid EmployeeId { get;  set; }
    public string PersonName { get;  set; } = null!;
    public string Mobile { get;  set; } = null!;
    public string Email { get;  set; } = null!;
    public string Address { get;  set; } = null!;
    public string? Occupation { get;  set; }
    public string? HowDoYouKnow { get;  set; }
}
