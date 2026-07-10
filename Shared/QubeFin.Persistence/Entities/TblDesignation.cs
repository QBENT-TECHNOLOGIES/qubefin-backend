using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblDesignation
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid PostId { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblOrganizationUnit OrganizationUnit { get; set; } = null!;

    public virtual TblPost Post { get; set; } = null!;

    public virtual ICollection<TblDesignationRole> TblDesignationRoles { get; set; } = new List<TblDesignationRole>();

    public virtual ICollection<TblEmployeeDesignation> TblEmployeeDesignations { get; set; } = new List<TblEmployeeDesignation>();

    public virtual ICollection<TblGroup> TblGroups { get; set; } = new List<TblGroup>();

    public virtual ICollection<TblMember> TblMembers { get; set; } = new List<TblMember>();

    public virtual ICollection<TblPayRoll> TblPayRolls { get; set; } = new List<TblPayRoll>();
}
