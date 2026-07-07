using QubeFin.Persistence.Models.App;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class Attendance
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
        public Attendance()
        {

        }
        public Attendance(Guid id, Guid employeeId, Guid? organizationUnitId, DateOnly attendanceDate, TimeOnly? expectedInTime, TimeOnly? expectedOutTime, TimeOnly actualInTime, TimeOnly? actualOutTime, bool isEarlyLeave, bool isLateEntry)
        {
            Id = id;
            EmployeeId = employeeId;
            OrganizationUnitId = organizationUnitId;
            AttendanceDate = attendanceDate;
            ExpectedInTime = expectedInTime;
            ExpectedOutTime = expectedOutTime;
            ActualInTime = actualInTime;
            ActualOutTime = actualOutTime;
            IsEarlyLeave = isEarlyLeave;
            IsLateEntry = isLateEntry;
        }
        public static Attendance Create(Guid id, Guid employeeId, Guid? organizationUnitId, DateOnly attendanceDate, TimeOnly? expectedInTime, TimeOnly? expectedOutTime, TimeOnly actualInTime, TimeOnly? actualOutTime, bool isEarlyLeave, bool isLateEntry)
        {
            var attendance = new Attendance()
            {
                Id = id,
                EmployeeId = employeeId,
                ActualInTime = actualInTime,
                ActualOutTime = actualOutTime,
                AttendanceDate = attendanceDate,
                ExpectedInTime = expectedInTime,
                ExpectedOutTime = expectedOutTime,
                IsEarlyLeave = isEarlyLeave,
                IsLateEntry = isLateEntry,
                OrganizationUnitId = organizationUnitId
            };

            return attendance;
        }

    }
}
