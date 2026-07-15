namespace QubeFin.Hrms.Api.Requests;

public class PersonalInfoRequest
{
    public string Code { get;  set; } = null!;
    public string? Salutation { get;  set; }
    public string FirstName { get;  set; } = null!;
    public string? MiddleName { get;  set; }
    public string LastName { get;  set; } = null!;
    public string? FatherName { get;  set; }
    public string? MotherName { get;  set; }
    public DateTime DateOfBirth { get;  set; }
    public string Gender { get;  set; } = null!;
    public string Religion { get;  set; } = null!;
    public string? Caste { get;  set; }
    public string Nationality { get;  set; } = null!;
    public string BloodGroup { get;  set; } = null!;
    public string? DisablityType { get;  set; }
    public string? MaritalStatus { get;  set; }
}
