using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public PersonalInformation PersonalInformation { get; private set; } = default!;
        public OfficialInformation OfficialInformation { get; private set; } = default!;
        public AddressInfo PresentAddressInformation { get; private set; } = default!;
        public AddressInfo PermanentAddressInfo { get; private set; } = default!;

        //private readonly List<EmployeeQualification> _qualifications = [];



        public string MobileNo { get; private set; } = null!;

        public string? PersonalEmail { get; private set; }

        public string? EmergencyContactRelation1 { get; private set; }

        public string? EmergencyContactName1 { get; private set; }

        public string? EmergencyContactMobile1 { get; private set; }

        public string? EmergencyContactRelation2 { get; private set; }

        public string? EmergencyContactName2 { get; private set; }

        public string? EmergencyContactMobile2 { get; private set; }


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



        public Guid? CreatedBy { get; private set; }
        public DateTime? CreatedDate { get; private set; }
        public DateTime? LastModifiedOn { get; private set; }
        public Guid? LastModifiedBy { get; private set; }
        public TimeOnly? ExpectedInTime { get; private set; }
        public TimeOnly? ExpectedOutTime { get; private set; }

    }    
}
