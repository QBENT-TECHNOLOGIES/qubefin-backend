using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IAttendanceRepository
    {
        Task Create(Attendance attendance);
        Task<Attendance?> GetTodayAttendanceData(Guid EmployeeId);
        Task Update(Attendance attendance);
        Task<OrganizationInfo?> GetOrganization(Guid OrganizationUnitId);
    }
    public class AttendanceRepository(QubeFinDataContext context) : IAttendanceRepository
    {
        public Task Create(Attendance attendance)
        {
            context.TblAttendances.Add(attendance.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<Attendance?> GetTodayAttendanceData(Guid EmployeeId)
        {
            var attendanceEntity = await context.TblAttendances.AsNoTracking().FirstOrDefaultAsync(m => m.EmployeeId == EmployeeId && m.AttendanceDate == DateOnly.FromDateTime(DateTime.Now));
            if (attendanceEntity is null)
            {
                return null;
            }

            return attendanceEntity.ToDomain();
        }
        public Task Update(Attendance attendance)
        {
            context.TblAttendances.Update(attendance.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<OrganizationInfo?> GetOrganization(Guid OrganizationUnitId)
        {
            var org = await context.TblOrganizationUnits.AsNoTracking().FirstOrDefaultAsync(m => m.Id == OrganizationUnitId);
            if (org is null)
            {
                return null;
            }
            return new OrganizationInfo
            {
                Id = org.Id,
                Name = org.Name,
                Latitude = org.Latitude,
                Longitude = org.Longitude,
                AttendanceInTime = org.AttendanceInTime,
                AttendanceOutTime = org.AttendanceOutTime,
                CheckRadiusInMeter = org.CheckRadiusInMeter
            };
        }
    }
    public class OrganizationInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? AttendanceInTime { get; set; }
        public TimeOnly? AttendanceOutTime { get; set; }
        public int? CheckRadiusInMeter { get; set; }
    }
}
