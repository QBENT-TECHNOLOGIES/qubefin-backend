using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployee
{
    public Guid Id { get; set; }

    public string? Salutation { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? FatherName { get; set; }

    public string? MotherName { get; set; }

    public Guid? OrganizationUnitId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? EmployementType { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public DateOnly? JoiningDate { get; set; }

    public DateOnly? ConfirmationDate { get; set; }

    public DateOnly? SeparationDate { get; set; }

    public string Gender { get; set; } = null!;

    public string Religion { get; set; } = null!;

    public string? Caste { get; set; }

    public string Nationality { get; set; } = null!;

    public string BloodGroup { get; set; } = null!;

    public string? DisablityType { get; set; }

    public string? MaritalStatus { get; set; }

    public string MobileNo { get; set; } = null!;

    public string? PersonalEmail { get; set; }

    public string? EmergencyContactRelation1 { get; set; }

    public string? EmergencyContactName1 { get; set; }

    public string? EmergencyContactMobile1 { get; set; }

    public string? EmergencyContactRelation2 { get; set; }

    public string? EmergencyContactName2 { get; set; }

    public string? EmergencyContactMobile2 { get; set; }

    public string? PermanentHouseNo { get; set; }

    public string? PermanentRoadName { get; set; }

    public string? PermanentLandMark { get; set; }

    public Guid? PermanentAdministrativeUnitId { get; set; }

    public Guid? PermanentPoliceStationId { get; set; }

    public Guid? PermanentPostOfficeId { get; set; }

    public string? PermanentPinCode { get; set; }

    public string? PermanentOwnerShipOfHouse { get; set; }

    public int? PermanentDurationOfStayInMonths { get; set; }

    public string? PresentHouseNo { get; set; }

    public string? PresentRoadName { get; set; }

    public string? PresentLandMark { get; set; }

    public Guid? PresentAdministrativeUnitId { get; set; }

    public Guid? PresentPoliceStationId { get; set; }

    public Guid? PresentPostOfficeId { get; set; }

    public string? PresentPinCode { get; set; }

    public string? PresentOwnerShipOfHouse { get; set; }

    public int? PresentDurationOfStayInMonths { get; set; }

    public Guid? BankId { get; set; }

    public long? BankAccountNo { get; set; }

    public string? BankHolderName { get; set; }

    public string? BankBranch { get; set; }

    public string? BankAccountType { get; set; }

    public string? OfficialEmail { get; set; }

    public bool IsActive { get; set; }

    public bool IsPayrollActive { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? ReferedBy { get; set; }

    public string? HowYouKnow { get; set; }

    public string? UniversalAccountNo { get; set; }

    public string? Esiipno { get; set; }

    public bool HasEsiEligible { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblFinancialInstitute? Bank { get; set; }

    public virtual TblCompany? Company { get; set; }

    public virtual TblDepartment? Department { get; set; }

    public virtual TblOrganizationUnit? OrganizationUnit { get; set; }

    public virtual TblAdministrativeUnit? PermanentAdministrativeUnit { get; set; }

    public virtual TblAdministrativeUnit? PresentAdministrativeUnit { get; set; }

    public virtual ICollection<TblEmployeeDesignation> TblEmployeeDesignations { get; set; } = new List<TblEmployeeDesignation>();

    public virtual ICollection<TblEmployeeDocument> TblEmployeeDocuments { get; set; } = new List<TblEmployeeDocument>();

    public virtual ICollection<TblEmployeeEmployment> TblEmployeeEmployments { get; set; } = new List<TblEmployeeEmployment>();

    public virtual ICollection<TblEmployeeQualification> TblEmployeeQualifications { get; set; } = new List<TblEmployeeQualification>();

    public virtual ICollection<TblEmployeeReference> TblEmployeeReferences { get; set; } = new List<TblEmployeeReference>();

    public virtual ICollection<TblLeaveTransaction> TblLeaveTransactions { get; set; } = new List<TblLeaveTransaction>();

    public virtual ICollection<TblPayRoll> TblPayRolls { get; set; } = new List<TblPayRoll>();

    public virtual ICollection<TblUser> TblUsers { get; set; } = new List<TblUser>();
}
