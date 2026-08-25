using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblEmployeeLop
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public int LopMonth { get; set; }

    public int LopYear { get; set; }

    public int HoliDays { get; set; }

    public int WorkingDays { get; set; }

    public int LeaveDays { get; set; }

    public int AttendanceDays { get; set; }

    public int AbsentDays { get; set; }

    public int AttendanceIrregularDays { get; set; }

    public int IrregularLopDays { get; set; }

    public bool IsLocked { get; set; }

    public string? Remarks { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual TblEmployee Employee { get; set; } = null!;

    public virtual TblOrganizationUnit OrganizationUnit { get; set; } = null!;

    public virtual ICollection<TblEmployeeLopDetail> TblEmployeeLopDetails { get; set; } = new List<TblEmployeeLopDetail>();
}
