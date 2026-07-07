using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IAttendanceRepository
    {
    }
    public class AuthRepository(QubeFinDataContext context) : IAttendanceRepository
    {
        public Task CreateAttendance(Attendance attendance)
        {
            context.TblAttendances.Add(attendance.ToEntity());
            return Task.CompletedTask;
        }
    }
}
