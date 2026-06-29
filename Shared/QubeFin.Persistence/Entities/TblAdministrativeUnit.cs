using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAdministrativeUnit
{
    public Guid Id { get; set; }

    public Guid AdministrativeUnitTypeId { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblAdministrativeUnitType AdministrativeUnitType { get; set; } = null!;

    public virtual ICollection<TblAdministrativeUnit> InverseParent { get; set; } = new List<TblAdministrativeUnit>();

    public virtual TblAdministrativeUnit? Parent { get; set; }
}
