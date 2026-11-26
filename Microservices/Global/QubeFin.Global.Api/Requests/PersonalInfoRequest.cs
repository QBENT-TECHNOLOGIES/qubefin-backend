namespace QubeFin.Global.Api.Requests;

public class PersonalInfoRequest
{
    public string Code { get; set; } = null!;
    public int BranchCode { get; set; }
    public Guid? GroupId { get; set; }
    //public string? GroupCode { get; set; }
    //public string? GroupName { get; set; }
    public string? Salutaion { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? GuardianName { get; set; }
    public string? MotherName { get; set; }
    public string? SpouseName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Religion { get; set; } = null!;
    public string? Caste { get; set; }
    public string Nationality { get; set; } = null!;
    public string BloodGroup { get; set; } = null!;
    public string? DisablityType { get; set; }
    public string? MaritalStatus { get; set; }
    public string? MobileNo { get; set; }
    public string? AlternateMobileNo { get; set; }
    public string? EducationalQualification { get; set; }
}