using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeTransfer
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid OrganisationUnitId { get; set; }

    public Guid DesignationId { get; set; }

    public decimal GrossSalary { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public bool IsApprove { get; set; }

    public virtual TblDesignation Designation { get; set; } = null!;

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblOrganizationUnit OrganisationUnit { get; set; } = null!;
}
