using Microsoft.IdentityModel.Tokens;

namespace QubeFin.Persistence.Models.Hrms;

public class PersonalInfo
{
    public string Code { get; private set; } = null!;
    public string? Salutation { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = null!;
    public string? FatherName { get; private set; }
    public string? MotherName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = null!;
    public string Religion { get; private set; } = null!;
    public string? Caste { get; private set; }
    public string Nationality { get; private set; } = null!;
    public string BloodGroup { get; private set; } = null!;
    public string? DisablityType { get; private set; }
    public string? MaritalStatus { get; private set; }

    public PersonalInfo()
    {
    }

    public PersonalInfo(string code, string? salutation, string firstName, string? middleName, string lastName, string? fatherName, string? motherName,
        DateOnly dateOfBirth, string gender, string religion, string? caste, string nationality, string bloodGroup, string? disablityType, string? maritalStatus)
    {
        Code = code;
        Salutation = salutation;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FatherName = fatherName;
        MotherName = motherName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Religion = religion;
        Caste = caste;
        Nationality = nationality;
        BloodGroup = bloodGroup;
        DisablityType = disablityType;
        MaritalStatus = maritalStatus?.Trim();
    }
}
