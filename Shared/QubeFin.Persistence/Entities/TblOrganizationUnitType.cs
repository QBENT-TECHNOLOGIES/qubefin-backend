using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblOrganizationUnitType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public int LevelNo { get; set; }

    public virtual ICollection<TblApprovalWorkflowEvent> TblApprovalWorkflowEvents { get; set; } = new List<TblApprovalWorkflowEvent>();

    public virtual ICollection<TblApprovalWorkflow> TblApprovalWorkflows { get; set; } = new List<TblApprovalWorkflow>();

    public virtual ICollection<TblOrganizationUnit> TblOrganizationUnits { get; set; } = new List<TblOrganizationUnit>();
}
