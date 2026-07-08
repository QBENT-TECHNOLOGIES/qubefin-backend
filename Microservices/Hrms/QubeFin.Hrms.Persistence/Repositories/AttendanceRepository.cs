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
        Task Create(Attendance attendance);
        Task<Attendance> GetTodayAttendanceData(Guid EmployeeId);
        Task Update(Attendance attendance);
    }
    public class AttendanceRepository(QubeFinDataContext context) : IAttendanceRepository
    {
        public Task Create(Attendance attendance)
        {
            context.TblAttendances.Add(attendance.ToEntity());
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

        public Task Update(Attendance attendance)
        {
            context.TblAttendances.Update(attendance.ToEntity());
            return Task.CompletedTask;
        }


    }
}
