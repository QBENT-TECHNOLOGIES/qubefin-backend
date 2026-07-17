using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblCompany
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TblEmployee> TblEmployees { get; set; } = new List<TblEmployee>();

    public virtual ICollection<TblOrganizationUnit> TblOrganizationUnits { get; set; } = new List<TblOrganizationUnit>();

    public virtual ICollection<TblPayRoll> TblPayRolls { get; set; } = new List<TblPayRoll>();
}
