using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global;

public class OrganizationUnitHierarchyItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Level { get; set; }
}
