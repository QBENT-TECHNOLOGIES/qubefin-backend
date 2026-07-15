namespace QubeFin.Hrms.Api.Requests;

public class ContactInfoRequest
{
    public required string MobileNo { get;  set; } 
    public string? PersonalEmail { get;  set; }
    public string? PrimaryEmergencyRelation { get;  set; }
    public string? PrimaryEmergencyName { get;  set; }
    public string? PrimaryEmergencyMobile { get;  set; }
    public string? SecondaryEmergencyRelation { get;  set; }
    public string? SecondaryEmergencyName { get;  set; }
    public string? SecondaryEmergencyMobile { get;  set; }
}
