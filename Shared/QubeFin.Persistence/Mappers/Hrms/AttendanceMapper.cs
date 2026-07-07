using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblAttendance;

namespace QubeFin.Persistence.Mappers.App;

public static class AttendanceMapper
{
    public static Attendance ToDomain(this Entity entity)
    {
        return new Attendance(
            entity.Id,
            entity.EmployeeId,
            entity.OrganizationUnitId,
            entity.AttendanceDate,
            entity.ExpectedInTime,
            entity.ExpectedOutTime,
            entity.ActualInTime,
            entity.ActualOutTime,
            entity.IsEarlyLeave,
            entity.IsLateEntry
            );
    }

    public static Entity ToEntity(this Attendance domain)
    {
        return new Entity
        {
            Id = domain.Id,
            EmployeeId = domain.EmployeeId,
            OrganizationUnitId = domain.OrganizationUnitId,
            ExpectedInTime = domain.ExpectedInTime,
            ExpectedOutTime  = domain.ExpectedOutTime,
            ActualInTime = domain.ActualInTime,
            ActualOutTime = domain.ActualOutTime,
            AttendanceDate = domain.AttendanceDate,
            IsEarlyLeave = domain.IsEarlyLeave,
            IsLateEntry = domain.IsLateEntry
        };
    }
}