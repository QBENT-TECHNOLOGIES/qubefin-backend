using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblPayRoll
{
    public Guid Id { get; set; }

    public int PayrollMonth { get; set; }

    public int PayrollYear { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public Guid DesignationId { get; set; }

    public Guid CompanyId { get; set; }

    public bool IsLocked { get; set; }

    public virtual TblCompany Company { get; set; } = null!;

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblOrganizationUnit IdNavigation { get; set; } = null!;
}
