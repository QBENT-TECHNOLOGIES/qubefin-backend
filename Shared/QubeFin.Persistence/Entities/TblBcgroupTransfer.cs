using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblBcgroupTransfer
{
    public Guid Id { get; set; }

    public Guid BcgroupId { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public Guid DesignationId { get; set; }

    public DateOnly EffectedFrom { get; set; }

    public DateOnly? EffectedTo { get; set; }

    public virtual TblBcgroup Bcgroup { get; set; } = null!;

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblOrganizationUnit OrganizationUnit { get; set; } = null!;
}
