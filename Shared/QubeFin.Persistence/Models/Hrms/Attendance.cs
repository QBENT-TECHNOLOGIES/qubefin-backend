using QubeFin.Persistence.Models.App;

namespace QubeFin.Persistence.Models.Hrms;

public class Attendance
{
    public Guid Id { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Guid? OrganizationUnitId { get; private set; }
    public DateOnly AttendanceDate { get; private set; }
    public TimeOnly? ExpectedInTime { get; private set; }
    public TimeOnly? ExpectedOutTime { get; private set; }
    public TimeOnly ActualInTime { get; private set; }
    public TimeOnly? ActualOutTime { get; private set; }
    public bool IsEarlyLeave { get; private set; }
    public bool IsLateEntry { get; private set; }
    public decimal? InTimeLatitude {  get; private set; }
    public decimal? InTimeLongitude { get; private set; }
    public decimal? OutTimeLatitude { get; private set; }
    public decimal? OutTimeLongitude { get; private set; }
    public Attendance()
    {

    }
    public Attendance(Guid id, Guid employeeId, Guid? organizationUnitId, DateOnly attendanceDate, TimeOnly? expectedInTime, TimeOnly? expectedOutTime, TimeOnly actualInTime, TimeOnly? actualOutTime, bool isEarlyLeave, bool isLateEntry, decimal? intimeLat, decimal? intimeLong, decimal? outTimeLat, decimal? outTimeLong)
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
        InTimeLatitude = intimeLat;
        InTimeLongitude = intimeLong;
        OutTimeLatitude = outTimeLat;
        OutTimeLongitude = outTimeLong;
    }
    public static Attendance MarkCheckIn(Guid id, Guid employeeId,  TimeOnly InTime, TimeOnly? OutTime, Guid? organizationUnitId, TimeOnly? expectedInTime, TimeOnly? expectedOutTime, decimal? InTimeLat, decimal? InTimeLong, decimal? OutTimeLat, decimal? OutTimeLong, DateOnly attendanceDate)
    {
        var attendance = new Attendance()
        {
            Id = id,
            EmployeeId = employeeId,
            ActualInTime = InTime,
            ActualOutTime = OutTime,
            OrganizationUnitId = organizationUnitId,
            ExpectedInTime = expectedInTime,
            ExpectedOutTime = expectedOutTime,
            InTimeLatitude = InTimeLat,
            InTimeLongitude = InTimeLong,
            OutTimeLatitude = OutTimeLat,
            OutTimeLongitude = OutTimeLong,
            AttendanceDate = attendanceDate,
        };

        return attendance;
    }
    public void MarchCheckOut(TimeOnly? outTime, decimal? outTimeLat, decimal? outTimeLong)
    {
        ActualOutTime = outTime;
        OutTimeLatitude = outTimeLat;
        OutTimeLongitude = outTimeLong;
    }
}
