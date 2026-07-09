using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblMember
{
    public Guid Id { get; set; }

    public int BranchCode { get; set; }

    public string? Code { get; set; }

    public int CodeVal { get; set; }

    public string? Salutaion { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string FullName { get; set; } = null!;

    public string? GuardianRelation { get; set; }

    public string? GuardianName { get; set; }

    public string? MotherName { get; set; }

    public string? SpouseName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Religion { get; set; }

    public string? Caste { get; set; }

    public string? Qualification { get; set; }

    public string? Occupation { get; set; }

    public string MobileNo { get; set; } = null!;

    public bool IsMobileVerified { get; set; }

    public string AadharNo { get; set; } = null!;

    public bool IsAadharVerified { get; set; }

    public string? PanNo { get; set; }

    public bool IsPanVerified { get; set; }

    public string? CkycNumber { get; set; }

    public Guid? GroupId { get; set; }

    public Guid? DesignationId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblDesignation? Designation { get; set; }

    public virtual TblGroup? Group { get; set; }

    public virtual ICollection<TblCoBorrower> TblCoBorrowers { get; set; } = new List<TblCoBorrower>();

    public virtual ICollection<TblCreditDatum> TblCreditData { get; set; } = new List<TblCreditDatum>();

    public virtual ICollection<TblLoanApplication> TblLoanApplications { get; set; } = new List<TblLoanApplication>();

    public virtual ICollection<TblMemberAddress> TblMemberAddresses { get; set; } = new List<TblMemberAddress>();

    public virtual ICollection<TblMemberDocument> TblMemberDocuments { get; set; } = new List<TblMemberDocument>();
}
