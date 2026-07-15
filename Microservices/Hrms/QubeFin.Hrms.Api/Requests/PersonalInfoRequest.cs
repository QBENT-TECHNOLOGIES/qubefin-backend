namespace QubeFin.Hrms.Api.Requests;

public class PersonalInfoRequest
{
    public string Code { get; private set; } = null!;
    public string? Salutation { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = null!;
    public string? FatherName { get; private set; }
    public string? MotherName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Gender { get; private set; } = null!;
    public string Religion { get; private set; } = null!;
    public string? Caste { get; private set; }
    public string Nationality { get; private set; } = null!;
    public string BloodGroup { get; private set; } = null!;
    public string? DisablityType { get; private set; }
    public string? MaritalStatus { get; private set; }
}
