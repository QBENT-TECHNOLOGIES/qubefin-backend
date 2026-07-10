using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblGroup
{
    public Guid Id { get; set; }

    public int? BranchCode { get; set; }

    public string? Code { get; set; }

    public int CodeVal { get; set; }

    public string Name { get; set; } = null!;

    public int CollectionDay { get; set; }

    public TimeOnly CollectionTime { get; set; }

    public DateOnly? BiweeklyStartDate { get; set; }

    public DateOnly FormationDate { get; set; }

    public string MobileNo { get; set; } = null!;

    public Guid AdministrativeUnitId { get; set; }

    public string? PinCode { get; set; }

    public string? Address { get; set; }

    public string? LandMark { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? PhotoScan { get; set; }

    public bool IsApproved { get; set; }

    public string? CurrentStatus { get; set; }

    public Guid? VerifiedBy { get; set; }

    public DateTime? VerifiedOn { get; set; }

    public string? VerifiedRemarks { get; set; }

    public Guid? DesignationId { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblAdministrativeUnit AdministrativeUnit { get; set; } = null!;

    public virtual TblDesignation? Designation { get; set; }

    public virtual ICollection<TblMember> TblMembers { get; set; } = new List<TblMember>();
}
