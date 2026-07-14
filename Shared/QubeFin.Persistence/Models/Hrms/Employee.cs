using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms;

public class Employee
{
    public Guid Id { get; private set; }
    public string? Salutation { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public string? FatherName { get; private set; }
    public string? MotherName { get; private set; }
    public Guid? OrganizationUnitId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public string? EmployementType { get; private set; }
    public DateOnly? JoiningDate { get; private set; }
    public DateOnly? ConfirmationDate { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = null!;
    public string Religion { get; private set; } = null!;
    public string? Caste { get; private set; }
    public string Nationality { get; private set; } = null!;
    public string BloodGroup { get; private set; } = null!;
    public string? DisablityType { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string MobileNo { get; private set; } = null!;
    public string? PersonalEmail { get; private set; }
    public string? EmergencyContactRelation1 { get; private set; }
    public string? EmergencyContactName1 { get; private set; }
    public string? EmergencyContactMobile1 { get; private set; }
    public string? EmergencyContactRelation2 { get; private set; }
    public string? EmergencyContactName2 { get; private set; }
    public string? EmergencyContactMobile2 { get; private set; }
    public string? PermanentHouseNo { get; private set; }
    public string? PermanentRoadName { get; private set; }
    public string? PermanentLandMark { get; private set; }
    public Guid? PermanentAdministrativeUnitId { get; private set; }
    public Guid? PermanentPoliceStationId { get; private set; }
    public Guid? PermanentPostOfficeId { get; private set; }
    public string? PermanentPinCode { get; private set; }
    public string? PermanentOwnerShipOfHouse { get; private set; }
    public int? PermanentDurationOfStayInMonths { get; private set; }
    public string? PresentHouseNo { get; private set; }
    public string? PresentRoadName { get; private set; }
    public string? PresentLandMark { get; private set; }
    public Guid? PresentAdministrativeUnitId { get; private set; }
    public Guid? PresentPoliceStationId { get; private set; }
    public Guid? PresentPostOfficeId { get; private set; }
    public string? PresentPinCode { get; private set; }
    public string? PresentOwnerShipOfHouse { get; private set; }
    public int? PresentDurationOfStayInMonths { get; private set; }
    public Guid? BankId { get; private set; }
    public long? BankAccountNo { get; private set; }
    public string? BankHolderName { get; private set; }
    public string? BankBranch { get; private set; }
    public string? BankAccountType { get; private set; }
    public string? OfficialEmail { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsPayrollActive { get; private set; }
    public Guid? CompanyId { get; private set; }
    public DateOnly? SeparationDate { get; private set; }
    public Guid? ReferedBy { get; private set; }
    public string? HowYouKnow { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? CreatedDate { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }


    private readonly List<EmployeeDocument> _documents = [];
    public IReadOnlyCollection<EmployeeDocument> Documents =>
        _documents.AsReadOnly();

    private Employee() { }

    public Employee
    (
        Guid id,
        string? salutation,
        string firstName,
        string? middleName,
        string lastName,
        string code,
        string? fatherName,
        string? motherName,
        Guid? organizationUnitId,
        Guid? departmentId,
        string? employementType,
        DateOnly? joiningDate,
        DateOnly? confirmationDate,
        DateOnly dateOfBirth,
        string gender,
        string religion,
        string? caste,
        string nationality,
        string bloodGroup,
        string? disablityType,
        string? maritalStatus,
        string mobileNo,
        string? personalEmail,
        string? emergencyContactRelation1,
        string? emergencyContactName1,
        string? emergencyContactMobile1,
        string? emergencyContactRelation2,
        string? emergencyContactName2,
        string? emergencyContactMobile2,
        string? permanentHouseNo,
        string? permanentRoadName,
        string? permanentLandMark,
        Guid? permanentAdministrativeUnitId,
        Guid? permanentPoliceStationId,
        Guid? permanentPostOfficeId,
        string? permanentPinCode,
        string? permanentOwnerShipOfHouse,
        int? permanentDurationOfStayInMonths,
        string? presentHouseNo,
        string? presentRoadName,
        string? presentLandMark,
        Guid? presentAdministrativeUnitId,
        Guid? presentPoliceStationId,
        Guid? presentPostOfficeId,
        string? presentPinCode,
        string? presentOwnerShipOfHouse,
        int? presentDurationOfStayInMonths,
        Guid? bankId,
        long? bankAccountNo,
        string? bankHolderName,
        string? bankBranch,
        string? bankAccountType,
        string? officialEmail,
        bool isActive,
        bool isPayrollActive,
        Guid? companyId,
        DateOnly? separationDate,
        Guid? referedBy,
        string? howYouKnow,
        Guid? lastModifiedBy,
        DateTime? lastModifiedOn
    )
    {
        Id = id;
        Salutation = salutation;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Code = code;
        FatherName = fatherName;
        MotherName = motherName;
        OrganizationUnitId = organizationUnitId;
        DepartmentId = departmentId;
        EmployementType = employementType;
        JoiningDate = joiningDate;
        ConfirmationDate = confirmationDate;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Religion = religion;
        Caste = caste;
        Nationality = nationality;
        BloodGroup = bloodGroup;
        DisablityType = disablityType;
        MaritalStatus = maritalStatus;
        MobileNo = mobileNo;
        PersonalEmail = personalEmail;
        EmergencyContactRelation1 = emergencyContactRelation1;
        EmergencyContactName1 = emergencyContactName1;
        EmergencyContactMobile1 = emergencyContactMobile1;
        EmergencyContactRelation2 = emergencyContactRelation2;
        EmergencyContactName2 = emergencyContactName2;
        EmergencyContactMobile2 = emergencyContactMobile2;
        PermanentHouseNo = permanentHouseNo;
        PermanentRoadName = permanentRoadName;
        PermanentLandMark = permanentLandMark;
        PermanentAdministrativeUnitId = permanentAdministrativeUnitId;
        PermanentPoliceStationId = permanentPoliceStationId;
        PermanentPostOfficeId = permanentPostOfficeId;
        PermanentPinCode = permanentPinCode;
        PermanentOwnerShipOfHouse = permanentOwnerShipOfHouse;
        PermanentDurationOfStayInMonths = permanentDurationOfStayInMonths;
        PresentHouseNo = presentHouseNo;
        PresentRoadName = presentRoadName;
        PresentLandMark = presentLandMark;
        PresentAdministrativeUnitId = presentAdministrativeUnitId;
        PresentPoliceStationId = presentPoliceStationId;
        PresentPostOfficeId = presentPostOfficeId;
        PresentPinCode = presentPinCode;
        PresentOwnerShipOfHouse = presentOwnerShipOfHouse;
        PresentDurationOfStayInMonths = presentDurationOfStayInMonths;
        BankId = bankId;
        BankAccountNo = bankAccountNo;
        BankHolderName = bankHolderName;
        BankBranch = bankBranch;
        BankAccountType = bankAccountType;
        OfficialEmail = officialEmail;
        IsActive = isActive;
        IsPayrollActive = isPayrollActive;
        CompanyId = companyId;
        SeparationDate = separationDate;
        ReferedBy = referedBy;
        HowYouKnow = howYouKnow;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
    }
    public static Employee CreatePersonalDetails(
        Guid id,
        string? salutation,
        string firstName,
        string? middleName,
        string lastName,
        string code,
        string? fatherName,
        string? motherName,
        DateOnly dateOfBirth,
        string gender,
        string religion,
        string? caste,
        string nationality,
        string bloodGroup,
        string? disablityType,
        string? maritalStatus,
        string mobileNo,
        string? personalEmail,
        Guid createdBy
    )
    {
        var employee = new Employee
        {
            Id = id,
            Salutation = salutation,
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Code = code,

            FatherName = fatherName,
            MotherName = motherName,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Religion = religion,
            Caste = caste,
            Nationality = nationality,
            BloodGroup = bloodGroup,
            DisablityType = disablityType,
            MaritalStatus = maritalStatus,
            MobileNo = mobileNo,
            PersonalEmail = personalEmail,
            IsActive = false,
            IsPayrollActive = false,
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now
        };

        return employee;

    }

    public void UpdateEmployeePersonalDetails(
        string? salutation,
        string firstName,
        string? middleName,
        string lastName,
        string? fatherName,
        string? motherName,
        DateOnly dateOfBirth,
        string gender,
        string religion,
        string? caste,
        string nationality,
        string bloodGroup,
        string? disablityType,
        string? maritalStatus,
        string mobileNo,
        string? personalEmail,
        Guid lastModifiedBy
    )
    {
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
        MaritalStatus = maritalStatus;
        MobileNo = mobileNo;
        PersonalEmail = personalEmail;
        IsActive = false;
        IsPayrollActive = false;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = DateTime.Now;

    }
    public void UpdateEmployeeAddressDetails(
            
        string? permanentHouseNo,
        string? permanentRoadName,
        string? permanentLandMark,
        Guid? permanentAdministrativeUnitId,
        Guid? permanentPoliceStationId,
        Guid? permanentPostOfficeId,
        string? permanentPinCode,
        string? permanentOwnerShipOfHouse,
        int? permanentDurationOfStayInMonths,
        string presentHouseNo,
        string? presentRoadName,
        string? presentLandMark,
        Guid? presentAdministrativeUnitId,
        Guid? presentPoliceStationId,
        Guid? presentPostOfficeId,
        string? presentPinCode,
        string? presentOwnerShipOfHouse,
        int? presentDurationOfStayInMonths,


        string? emergencyContactRelation1,
        string? emergencyContactName1,
        string? emergencyContactMobile1,
        string? emergencyContactRelation2,
        string? emergencyContactName2,
        string? emergencyContactMobile2,
        Guid lastModifiedBy
    )
    {

        PermanentHouseNo = permanentHouseNo;
        PermanentRoadName = permanentRoadName;
        PermanentLandMark = permanentLandMark;
        PermanentAdministrativeUnitId = permanentAdministrativeUnitId;
        PermanentPoliceStationId = permanentPoliceStationId;
        PermanentPostOfficeId = permanentPostOfficeId;
        PermanentPinCode = permanentPinCode;
        PermanentOwnerShipOfHouse = permanentOwnerShipOfHouse;
        PermanentDurationOfStayInMonths = permanentDurationOfStayInMonths;

        PresentHouseNo = presentHouseNo;
        PresentRoadName = presentRoadName;
        PresentLandMark = presentLandMark;
        PresentAdministrativeUnitId = presentAdministrativeUnitId;
        PresentPoliceStationId = presentPoliceStationId;
        PresentPostOfficeId = presentPostOfficeId;
        PresentPinCode = presentPinCode;
        PresentOwnerShipOfHouse = presentOwnerShipOfHouse;
        PresentDurationOfStayInMonths = presentDurationOfStayInMonths;


        EmergencyContactRelation1 = emergencyContactRelation1;
        EmergencyContactName1 = emergencyContactName1;
        EmergencyContactMobile1 = emergencyContactMobile1;
        EmergencyContactRelation2 = emergencyContactRelation2;
        EmergencyContactName2 = emergencyContactName2;
        EmergencyContactMobile2 = emergencyContactMobile2;

        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = DateTime.Now;

    }
    public void UpdateEmployeeOfficialDetails(
            
        Guid organizationUnitId,
        Guid departmentId,
        string? employementType,
        DateOnly joiningDate,
        DateOnly? confirmationDate,
        DateOnly? separationDate,
        Guid? referedBy,
        string? howYouKnow,

        Guid bankId,
        long bankAccountNo,
        string bankHolderName,
        string bankBranch,
        string bankAccountType,

        Guid lastModifiedBy
    )
    {
        OrganizationUnitId = organizationUnitId;
        DepartmentId = departmentId;
        EmployementType = employementType;
        JoiningDate = joiningDate;
        ConfirmationDate = confirmationDate;
        SeparationDate = separationDate;
        ReferedBy = referedBy;
        HowYouKnow = howYouKnow;

        BankId = bankId;
        BankAccountNo = bankAccountNo;
        BankHolderName = bankHolderName;
        BankBranch = bankBranch;
        BankAccountType = bankAccountType;

        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = DateTime.Now;
    }


    public void UpdateDocuments(IEnumerable<EmployeeDocument> _documents, Guid modifiedBy)
        {
            ArgumentNullException.ThrowIfNull(_documents);

            var documentSet = _documents
                .Distinct()
                .ToHashSet();

            //RemoveMissingDocuments(documentSet);
            AddNewDocuments(documentSet, modifiedBy);

            LastModifiedBy = modifiedBy;
            LastModifiedOn = DateTime.UtcNow;
        }
    //private void RemoveMissingDocuments(HashSet<EmployeeDocument> documents)
    //{
    //    _documents.RemoveAll(x => !documents.Contains(x));
    //}
    private void AddNewDocuments(HashSet<EmployeeDocument> documents, Guid createdBy)
    {
        foreach (var doc in documents)
        {
            //if (_documents.Any(x => x.Id == doc.Id))
            //    continue;

            _documents.Add(
                EmployeeDocument.Create(
                    //doc.EmployeeId,
                    doc.DocumentCategory,
                    doc.DocumentName,
                    doc.DocumentNo,
                    doc.ValidFrom,
                    doc.ValidTill,
                    doc.FileName,
                    doc.FileNo,
                    createdBy));
        }
    }



}
public class EmployeeOrganizationTiming
{
    public Guid OrganizationUnitId { get; set; }
    public TimeOnly? AttendanceInTime { get; set; }
    public TimeOnly? AttendanceOutTime { get; set; }
}
