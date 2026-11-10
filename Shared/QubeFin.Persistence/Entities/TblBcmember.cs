using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcmember
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public string? FatherName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Religion { get; set; }

    public string? Caste { get; set; }

    public string? Qualification { get; set; }

    public string? AadharNo { get; set; }

    public string? Pan { get; set; }

    public string? VoterNo { get; set; }

    public string? MobileNo { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? PinCode { get; set; }

    public string? LocationPhoto { get; set; }

    public Guid? TaggedBy { get; set; }

    public DateTime? TaggedOn { get; set; }

    public Guid BcgroupId { get; set; }

    public bool IsOverdueMember { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public virtual TblBcgroup Bcgroup { get; set; } = null!;

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblUser? TaggedByNavigation { get; set; }

    public virtual ICollection<TblBcloan> TblBcloans { get; set; } = new List<TblBcloan>();

    public virtual ICollection<TblBcmemberTransfer> TblBcmemberTransfers { get; set; } = new List<TblBcmemberTransfer>();
}
