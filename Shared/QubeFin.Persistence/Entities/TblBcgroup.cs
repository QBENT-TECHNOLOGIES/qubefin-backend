using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcgroup
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? AddressLine1 { get; set; }

    public Guid? AddressLine2 { get; set; }

    public string? PinCode { get; set; }

    public string? LocationPhoto { get; set; }

    public Guid? TaggedBy { get; set; }

    public DateTime? TaggedOn { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public Guid DesignationId { get; set; }

    public DateOnly? FormationDate { get; set; }

    public string? ContactNo { get; set; }

    public string? CollectionDay { get; set; }

    public TimeOnly? CollectionTime { get; set; }

    public DateOnly? BiWeeklyStartDate { get; set; }

    public bool IsMonthlyGroup { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? CreatedBy { get; set; }

    public virtual TblUser? CreatedByNavigation { get; set; }

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblOrganizationUnit OrganizationUnit { get; set; } = null!;

    public virtual TblUser? TaggedByNavigation { get; set; }

    public virtual ICollection<TblBcgroupTransfer> TblBcgroupTransfers { get; set; } = new List<TblBcgroupTransfer>();

    public virtual ICollection<TblBcmemberTransfer> TblBcmemberTransfers { get; set; } = new List<TblBcmemberTransfer>();

    public virtual ICollection<TblBcmember> TblBcmembers { get; set; } = new List<TblBcmember>();
}
