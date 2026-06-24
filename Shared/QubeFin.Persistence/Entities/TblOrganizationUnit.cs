using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblOrganizationUnit
{
    public Guid Id { get; set; }

    public Guid OrganizationUnitTypeId { get; set; }

    public string Name { get; set; } = null!;

    public int CodeVal { get; set; }

    public Guid? ParentId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ICollection<TblOrganizationUnit> InverseParent { get; set; } = new List<TblOrganizationUnit>();

    public virtual TblOrganizationUnitType OrganizationUnitType { get; set; } = null!;

    public virtual TblOrganizationUnit? Parent { get; set; }

    public virtual ICollection<TblDesignation> TblDesignations { get; set; } = new List<TblDesignation>();
}
