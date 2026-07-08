using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Reflection.Metadata.Ecma335;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IAttendanceRepository
    {
        Task CreateAttendance(Attendance attendance);
        Task<Attendance> GetTodayAttendanceData(Guid EmployeeId);
    }
    public class AuthRepository(QubeFinDataContext context) : IAttendanceRepository
    {
        public Task CreateAttendance(Attendance attendance)
        {
            var employeeOrganizationUnit = context.TblEmployees.Include(m => m.OrganizationUnit).Single(m => m.Id == attendance.EmployeeId);

            var newAttendance = Attendance.MarkCheckIn(Guid.NewGuid(), attendance.EmployeeId, attendance.ActualInTime, employeeOrganizationUnit.OrganizationUnitId, employeeOrganizationUnit.OrganizationUnit?.AttendanceInTime, employeeOrganizationUnit.OrganizationUnit?.AttendanceOutTime);

            context.TblAttendances.Add(newAttendance.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<Attendance> GetTodayAttendanceData(Guid EmployeeId)
        {
            var attendanceEntity = await context.TblAttendances.FirstOrDefaultAsync(m => m.EmployeeId == EmployeeId && m.AttendanceDate == DateOnly.FromDateTime(DateTime.Now));
            if (attendanceEntity is null)
            {
                return null;
            }

            return attendanceEntity.ToDomain();
        }
    }
}
