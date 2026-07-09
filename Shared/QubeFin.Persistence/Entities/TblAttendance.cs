using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAttendance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid? OrganizationUnitId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public TimeOnly? ExpectedInTime { get; set; }

    public TimeOnly? ExpectedOutTime { get; set; }

    public TimeOnly ActualInTime { get; set; }

    public TimeOnly? ActualOutTime { get; set; }

    public bool IsEarlyLeave { get; set; }

    public bool IsLateEntry { get; set; }

    public decimal? InTimeLat { get; set; }

    public decimal? InTimeLong { get; set; }

    public decimal? OutTimeLat { get; set; }

    public decimal? OutTimeLong { get; set; }

    public virtual TblOrganizationUnit? OrganizationUnit { get; set; }
}
