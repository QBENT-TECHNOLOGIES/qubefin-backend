using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAdministrativeUnit
{
    public Guid Id { get; set; }

    public Guid AdministrativeUnitTypeId { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ParentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblAdministrativeUnitType AdministrativeUnitType { get; set; } = null!;

    public virtual ICollection<TblAdministrativeUnit> InverseParent { get; set; } = new List<TblAdministrativeUnit>();

    public virtual TblAdministrativeUnit? Parent { get; set; }

    public virtual ICollection<TblEmployee> TblEmployeePermanentAdministrativeUnits { get; set; } = new List<TblEmployee>();

    public virtual ICollection<TblEmployee> TblEmployeePresentAdministrativeUnits { get; set; } = new List<TblEmployee>();

    public virtual ICollection<TblGroup> TblGroups { get; set; } = new List<TblGroup>();

    public virtual ICollection<TblPoliceStation> TblPoliceStations { get; set; } = new List<TblPoliceStation>();
}
