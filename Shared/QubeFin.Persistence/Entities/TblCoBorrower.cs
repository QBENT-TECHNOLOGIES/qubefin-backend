using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCoBorrower
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? MobileNo { get; set; }

    public bool IsMobileVerified { get; set; }

    public string AadharNo { get; set; } = null!;

    public bool IsAadharVerified { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblMember Member { get; set; } = null!;

    public virtual ICollection<TblCreditDatum> TblCreditData { get; set; } = new List<TblCreditDatum>();

    public virtual ICollection<TblLoanApplication> TblLoanApplications { get; set; } = new List<TblLoanApplication>();

    public virtual ICollection<TblMemberDocument> TblMemberDocuments { get; set; } = new List<TblMemberDocument>();
}
