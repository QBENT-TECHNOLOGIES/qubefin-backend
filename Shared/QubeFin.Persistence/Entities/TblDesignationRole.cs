using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblDesignationRole
{
    public Guid Id { get; set; }

    public Guid DesignationId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblRole Role { get; set; } = null!;
}
