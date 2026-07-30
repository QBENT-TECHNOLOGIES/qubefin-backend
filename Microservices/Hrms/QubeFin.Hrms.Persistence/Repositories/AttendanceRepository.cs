using Microsoft.AspNetCore.Http;
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
        Task CreateRegularization(AttendanceRegularization regularization);
        Task<AttendanceRegularization?> GetRegularization(Guid Id);
        //Task UpdateRegularization(Guid Id, DateOnly RegularizationDate, string Reason, IFormFile? Attachment, string? AttachmentName);
        Task SubmitAttendanceRegularization(Guid Id, Guid EmployeeId);
        //Task ApproveRejectAttendanceRegularization(Guid Id, bool IsApproved, Guid ActionBy);
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
        public Task CreateRegularization(AttendanceRegularization regularization)
        {
            //context.TblAttendanceRegularizations.Add(regularization.ToEntity());
            return Task.CompletedTask;
        }
        //public async Task UpdateRegularization(Guid Id, DateOnly RegularizationDate, string Reason, IFormFile? Attachment, string? AttachmentName)
        //{
        //    var regularizationEntity = await context.TblAttendanceRegularizations.AsNoTracking().FirstOrDefaultAsync(m => m.Id == Id);
        //    if (regularizationEntity is null)
        //    {
        //        throw new Exception("Regularization not found");
        //    }
        //    regularizationEntity.RegularizationDate = RegularizationDate;
        //    regularizationEntity.Reason = Reason;
        //    if (Attachment != null && Attachment.Length > 0)
        //    {
        //        regularizationEntity.Attachment = AttachmentName;
        //    }
        //}
        public async Task<AttendanceRegularization?> GetRegularization(Guid Id)
        {
            var regularizationEntity = await context.TblAttendanceRegularizations.AsNoTracking().FirstOrDefaultAsync(m => m.Id == Id);
            if (regularizationEntity is null)
            {
               
            }
            return null;
            //return regularizationEntity.ToDomain();
        }
        public async Task SubmitAttendanceRegularization(Guid Id, Guid EmployeeId)
        {
            var entity = await context.TblAttendanceRegularizations.FirstOrDefaultAsync(x => x.Id == Id);
            if (entity is null)
            {
                throw new Exception("Attendance regularization not found");
            }
        }
        //public async Task ApproveRejectAttendanceRegularization(Guid Id, bool IsApproved, Guid ActionBy)
        //{
        //    var entity = await context.TblAttendanceRegularizations.FirstOrDefaultAsync(x => x.Id == Id);
        //    if (entity is null)
        //    {
        //        throw new Exception("Attendance regularization not found");
        //    }
        //    if (!entity.IsSubmit)
        //    {
        //        throw new Exception("Regularization has not been submitted.");
        //    }
        //    if (IsApproved)
        //    {
        //        entity.IsApproved = true;
        //        entity.IsRejected = false;
        //    }
        //    else
        //    {
        //        entity.IsRejected = true;
        //        entity.IsApproved = false;
        //    }
        //    entity.ActivityBy = ActionBy;
        //    entity.ActivityOn = DateTime.UtcNow;
        //    await context.SaveChangesAsync();
        //}
    }
}
