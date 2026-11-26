namespace QubeFin.Persistence.Models.Members;

public class PersonalInfo
{
    public int BranchCode { get; private set; }
    public Guid? GroupId { get; private set; }
    public string? GroupCode { get; set; }
    public string? GroupName { get; set; }
    public string? Code { get; private set; }
    public string? Salutaion { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string? MiddleName { get; private set; }
    public string? LastName { get; private set; }
    public string? GuardianName { get; private set; }
    public string? MotherName { get; private set; }
    public string? SpouseName { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string? Gender { get; private set; }
    public string? Religion { get; private set; }
    public string? Caste { get; private set; }
    public string? Nationality { get; set; }
    public string? MaritalStatus { get; private set; }
    public string? EducationalQualification { get; private set; }
    public string MobileNo { get; private set; } = null!;
    public string? AlternateMobileNo { get; set; }

    public PersonalInfo()
    {
    }

    public PersonalInfo(
        int branchCode,
        Guid? groupId,
        string? code,
        string? salutaion,
        string firstName,
        string? middleName,
        string? lastName,
        string? guardianName,
        string? motherName,
        string? spouseName,
        DateOnly dateOfBirth,
        string? gender,
        string? religion,
        string? caste,
        string? nationality,    
        string? maritalStatus,
        string? educationalQualification,
        string mobileNo,
        string? alternateMobileNo)
    {
        BranchCode = branchCode;
        GroupId = groupId;
        Code = code;
        Salutaion = salutaion;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        GuardianName = guardianName;
        MotherName = motherName;
        SpouseName = spouseName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Religion = religion;
        Caste = caste;
        Nationality = nationality;
        MaritalStatus = maritalStatus?.Trim();
        EducationalQualification = educationalQualification?.Trim();
        MobileNo = mobileNo;
        AlternateMobileNo = alternateMobileNo;
    }
}