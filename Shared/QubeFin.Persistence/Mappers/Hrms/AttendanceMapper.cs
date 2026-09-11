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
            entity.AttendanceDate,
            entity.ExpectedInTime,
            entity.ExpectedOutTime,
            entity.ActualInTime,
            entity.ActualOutTime,
            entity.CheckinOrganizationUnitId,
            entity.CheckoutOrganizationUnitId,
            entity.IsEarlyLeave,
            entity.IsLateEntry,
            entity.InTimeLatitude,
            entity.InTimeLongitude,
            entity.OutTimeLatitude,
            entity.OutTimeLongitude
        );
    }

    public static Entity ToEntity(this Attendance domain)
    {
        return new Entity
        {
            Id = domain.Id,
            EmployeeId = domain.EmployeeId,
            ExpectedInTime = domain.ExpectedInTime,
            ExpectedOutTime = domain.ExpectedOutTime,
            ActualInTime = domain.ActualInTime,
            ActualOutTime = domain.ActualOutTime,
            AttendanceDate = domain.AttendanceDate,
            CheckinOrganizationUnitId = domain.CheckinOrganizationUnitId,
            CheckoutOrganizationUnitId = domain.CheckoutOrganizationUnitId,
            IsEarlyLeave = domain.IsEarlyLeave,
            IsLateEntry = domain.IsLateEntry,
            InTimeLatitude = domain.InTimeLatitude,
            InTimeLongitude = domain.InTimeLongitude,
            OutTimeLatitude = domain.OutTimeLatitude,
            OutTimeLongitude = domain.OutTimeLongitude,
        };
    }
}