namespace QubeFin.Persistence.Models.Hrms;

public class ContactInfo
{
    public string MobileNo { get; private set; } = string.Empty;
    public string? PersonalEmail { get; private set; } = string.Empty;
    public EmergencyContact PrimaryContact { get; private set; } = default!;
    public EmergencyContact SecondaryContact { get; private set; } = default!;

    public ContactInfo()
    {
    }

    public ContactInfo(string mobileNo, string? personalEmail, string? primaryEmergencyRelation, string? primaryEmergencyName, string? primaryEmergencyMobile,
        string? secondaryEmergencyRelation, string? secondaryEmergencyName, string? secondaryEmergencyMobile)
    {
        MobileNo = mobileNo;
        PersonalEmail = personalEmail;
        PrimaryContact = new EmergencyContact(primaryEmergencyRelation, primaryEmergencyName, primaryEmergencyMobile);
        SecondaryContact = new EmergencyContact(secondaryEmergencyRelation, secondaryEmergencyName, secondaryEmergencyMobile);
    }
}

public class EmergencyContact
{
    public string? Relation { get; private set; } = string.Empty;
    public string? Name { get; private set; } = string.Empty;
    public string? Mobile { get; private set; } = string.Empty;

    public EmergencyContact()
    {
    }

    public EmergencyContact(string? relation, string? name, string? mobile)
    {
        Relation = relation;
        Name = name;
        Mobile = mobile;
    }
};
