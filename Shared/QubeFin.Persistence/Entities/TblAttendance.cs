using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblAttendance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public TimeOnly InTime { get; set; }

    public TimeOnly? OutTime { get; set; }

    public bool HasShortAttendance { get; set; }
}
