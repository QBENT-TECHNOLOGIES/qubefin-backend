using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblEmployee;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class EmployeeMapper
{
    public static Employee ToDomain(this Entity entity)
    {
        return new Employee(
            entity.Id,
            entity.Code,
            MapPersonalInfo(entity),
            MapOfficialInfo(entity),
            MapContactInfo(entity),
            MapPresentAddressInfo(entity),
            MapPermanentAddressInfo(entity),
            MapPayrollInfo(entity),
            entity.CreatedBy,
            entity.CreatedDate,
            entity.LastModifiedBy,
            entity.LastModifiedOn
        );
    }

    public static Entity ToEntity(this Employee employee)
    {
        return new Entity
        {
            Id = employee.Id,
            Code = employee.Code,

            Salutation = employee.PersonalInfo.Salutation,
            FirstName = employee.PersonalInfo.FirstName,
            MiddleName = employee.PersonalInfo.MiddleName,
            LastName = employee.PersonalInfo.LastName,
            FatherName = employee.PersonalInfo.FatherName,
            MotherName = employee.PersonalInfo.MotherName,
            DateOfBirth = employee.PersonalInfo.DateOfBirth,
            Gender = employee.PersonalInfo.Gender,
            Religion = employee.PersonalInfo.Religion,
            Caste = employee.PersonalInfo.Caste,
            Nationality = employee.PersonalInfo.Nationality,
            BloodGroup = employee.PersonalInfo.BloodGroup,
            DisablityType = employee.PersonalInfo.DisablityType,
            MaritalStatus = employee.PersonalInfo.MaritalStatus,

            CompanyId = employee.OfficialInfo.CompanyId,
            OrganizationUnitId = employee.OfficialInfo.OrganizationUnitId,
            DepartmentId = employee.OfficialInfo.DepartmentId,
            EmployementType = employee.OfficialInfo.EmployementType,
            JoiningDate = employee.OfficialInfo.DateOfJoining,
            ConfirmationDate = employee.OfficialInfo.DateOfConfirmation,
            SeparationDate = employee.OfficialInfo.SeparationDate,
            ReferedBy = employee.OfficialInfo.ReferedBy,
            HowYouKnow = employee.OfficialInfo.HowYouKnow,
            OfficialEmail = employee.OfficialInfo.OfficialEmail,
            IsActive = employee.OfficialInfo.IsActive,

            MobileNo = employee.ContactInfo.MobileNo,
            PersonalEmail = employee.ContactInfo.PersonalEmail,

            EmergencyContactRelation1 = employee.ContactInfo.PrimaryContact.Relation,
            EmergencyContactName1 = employee.ContactInfo.PrimaryContact.Name,
            EmergencyContactMobile1 = employee.ContactInfo.PrimaryContact.Mobile,

            EmergencyContactRelation2 = employee.ContactInfo.SecondaryContact.Relation,
            EmergencyContactName2 = employee.ContactInfo.SecondaryContact.Name,
            EmergencyContactMobile2 = employee.ContactInfo.SecondaryContact.Mobile,

            PresentHouseNo = employee.PresentAddressInfo.HouseNo,
            PresentRoadName = employee.PresentAddressInfo.RoadName,
            PresentLandMark = employee.PresentAddressInfo.LandMark,
            PresentAdministrativeUnitId = employee.PresentAddressInfo.AdministrativeUnitId,
            PresentPoliceStationId = employee.PresentAddressInfo.PoliceStationId,
            PresentPostOfficeId = employee.PresentAddressInfo.PostOfficeId,
            PresentPinCode = employee.PresentAddressInfo.PinCode,
            PresentOwnerShipOfHouse = employee.PresentAddressInfo.OwnerShipOfHouse,
            PresentDurationOfStayInMonths = employee.PresentAddressInfo.DurationOfStayInMonths,

            PermanentHouseNo = employee.PermanentAddressInfo.HouseNo,
            PermanentRoadName = employee.PermanentAddressInfo.RoadName,
            PermanentLandMark = employee.PermanentAddressInfo.LandMark,
            PermanentAdministrativeUnitId = employee.PermanentAddressInfo.AdministrativeUnitId,
            PermanentPoliceStationId = employee.PermanentAddressInfo.PoliceStationId,
            PermanentPostOfficeId = employee.PermanentAddressInfo.PostOfficeId,
            PermanentPinCode = employee.PermanentAddressInfo.PinCode,
            PermanentOwnerShipOfHouse = employee.PermanentAddressInfo.OwnerShipOfHouse,
            PermanentDurationOfStayInMonths = employee.PermanentAddressInfo.DurationOfStayInMonths,

            BankId = employee.PayrollInfo.BankId,
            BankAccountNo = employee.PayrollInfo.BankAccountNo,
            BankHolderName = employee.PayrollInfo.BankHolderName,
            BankBranch = employee.PayrollInfo.BankBranch,
            BankAccountType = employee.PayrollInfo.BankAccountType,
            HasEsiEligible = employee.PayrollInfo.HasEsiEligible,
            Esiipno = employee.PayrollInfo.EsiIpNumber,
            UniversalAccountNo = employee.PayrollInfo.UniversalAccountNumber,
            IsPayrollActive = employee.PayrollInfo.IsPayrollActive,
        };
    }

    private static PersonalInfo MapPersonalInfo(TblEmployee entity)
    {
        return new PersonalInfo(
            entity.Salutation,
            entity.FirstName,
            entity.MiddleName,
            entity.LastName,
            entity.FatherName,
            entity.MotherName,
            entity.DateOfBirth,
            entity.Gender,
            entity.Religion,
            entity.Caste,
            entity.Nationality,
            entity.BloodGroup,
            entity.DisablityType,
            entity.MaritalStatus
        );
    }

    private static OfficialInfo MapOfficialInfo(TblEmployee entity)
    {
        return new OfficialInfo(
            entity.CompanyId,
            entity.OrganizationUnitId,
            entity.DepartmentId,
            entity.EmployementType,
            entity.JoiningDate,
            entity.ConfirmationDate,
            entity.SeparationDate,
            entity.ReferedBy,
            entity.HowYouKnow,
            entity.OfficialEmail,
            entity.IsActive
        );
    }

    private static ContactInfo MapContactInfo(TblEmployee entity)
    {
        return new ContactInfo(
            entity.MobileNo,
            entity.PersonalEmail,
            entity.EmergencyContactRelation1,
            entity.EmergencyContactName1,
            entity.EmergencyContactMobile1,
            entity.EmergencyContactRelation2,
            entity.EmergencyContactName2,
            entity.EmergencyContactMobile2
        );
    }

    private static AddressInfo MapPresentAddressInfo(TblEmployee entity)
    {
        return new AddressInfo(
            entity.PresentHouseNo,
            entity.PresentRoadName,
            entity.PresentLandMark,
            entity.PresentAdministrativeUnitId,
            entity.PresentPoliceStationId,
            entity.PresentPostOfficeId,
            entity.PresentPinCode,
            entity.PresentOwnerShipOfHouse,
            entity.PresentDurationOfStayInMonths
        );
    }

    private static AddressInfo MapPermanentAddressInfo(TblEmployee entity)
    {
        return new AddressInfo(
            entity.PermanentHouseNo,
            entity.PermanentRoadName,
            entity.PermanentLandMark,
            entity.PermanentAdministrativeUnitId,
            entity.PermanentPoliceStationId,
            entity.PermanentPostOfficeId,
            entity.PermanentPinCode,
            entity.PermanentOwnerShipOfHouse,
            entity.PermanentDurationOfStayInMonths
        );
    }

    private static PayrollInfo MapPayrollInfo(TblEmployee entity)
    {
        return new PayrollInfo(
            entity.BankId,
            entity.BankAccountNo,
            entity.BankHolderName,
            entity.BankBranch,
            entity.BankAccountType,
            entity.HasEsiEligible,
            entity.Esiipno,
            entity.UniversalAccountNo,
            entity.IsPayrollActive
        );
    }
}
