using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeEmployment
{
    public Guid Id { get; set; }

    public string EmployerName { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public decimal LastDrawnSalary { get; set; }

    public string? JobTitle { get; set; }

    public string? NocFileName { get; set; }

    public string? NocFileNo { get; set; }

    public string? ExpCertFileName { get; set; }

    public string? ExpCertFileNo { get; set; }

    public Guid EmployeeId { get; set; }

    public int Sequence { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;
}
