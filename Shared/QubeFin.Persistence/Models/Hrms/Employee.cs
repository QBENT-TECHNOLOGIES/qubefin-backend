using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class Employee
    {
        public Guid Id { get;  private set; }

        public string? Salutation { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string? MiddleName { get; private set; }

        public string LastName { get; private set; } = null!;

        public string Code { get; private set; } = null!;

        public string? FatherName { get; private set; }

        public string? MotherName { get; private set; }

        public Guid OrganizationUnitId { get; private set; }

        public Guid DepartmentId { get; private set; }

        public Guid DesignationId { get; private set; }

        public string? EmployementType { get; private set; }

        public DateOnly DateOfJoining { get; private set; }

        public DateOnly? DateOfConfirmation { get; private set; }

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

        public string PresentHouseNo { get; private set; } = null!;

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

        public bool? IsActive { get; private set; }

        public bool? IsPayrollActive { get; private set; }

        public Guid? CompanyId { get; private set; }

        public DateOnly? SeparationDate { get; private set; }

        public Guid? ReferedBy { get; private set; }

        public string? HowYouKnow { get; private set; }
        public Guid? CreatedBy { get; private set; }
        public DateTime? CreatedDate { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }

        private Employee() { }

        public Employee
        (
            Guid id,
            string salutation,
            Guid createdBy,
            DateTime createdDate,
            Guid? lastModifiedBy,
            DateTime? lastModifiedOn

        )
        {
            Id = id;
            Salutation = salutation;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
            LastModifiedOn = lastModifiedOn;
            LastModifiedBy = lastModifiedBy;
        }

        public static Employee Create(
            Guid id,
            string salutation,
            Guid createdBy
        )
        {
            var employee = new Employee
            {
                Id = id,
                Salutation = salutation,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };

            return employee;
        }

    }
}
