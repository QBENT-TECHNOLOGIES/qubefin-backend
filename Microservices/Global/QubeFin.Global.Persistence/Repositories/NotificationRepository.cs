using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllAsync(Guid employeeId);
        Task<bool> MarkAsReadAsync(Guid id);
        Task<bool> MarkAllReadAsync(Guid employeeId);
        Task<int> GetCountAsync(Guid employeeId);
        Task AddForEmployeesAsync(IEnumerable<Guid> employeeIds, string title, string message, string icon, Guid referenceId, string referenceType, string actionUrl, Guid createdBy);
    }
    public class NotificationRepository(QubeFinDataContext context) : INotificationRepository
    {
        public async Task<IEnumerable<Notification>> GetAllAsync(Guid employeeId)
        {
            var employeeDesignationId = context.TblEmployeeDesignations
                .Where(ed => ed.EmployeeId == employeeId)
                .OrderByDescending(ed => ed.EffectiveFrom)
                .FirstOrDefault()?.DesignationId;
            if (employeeDesignationId == null || employeeDesignationId == Guid.Empty)
            {
                return Enumerable.Empty<Notification>();
            }
            var entities = await context.TblNotifications.AsNoTracking()
                .Where(n => n.DesignationId == employeeDesignationId && (!n.IsRead || (n.IsRead && n.CreatedOn.Date == DateTime.UtcNow.Date)))
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
            return entities.Select(m => m.ToDomain());
        }
        public async Task<bool> MarkAsReadAsync(Guid id)
        {
            var entity = await context.TblNotifications.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.IsRead = true;
            entity.ReadDate = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MarkAllReadAsync(Guid employeeId)
        {
            var employeeDesignationId = context.TblEmployeeDesignations
                .Where(ed => ed.EmployeeId == employeeId)
                .OrderByDescending(ed => ed.EffectiveFrom)
                .FirstOrDefault()?.DesignationId;
            if (employeeDesignationId == null || employeeDesignationId == Guid.Empty)
            {
                return false;
            }
            var entities = await context.TblNotifications
                .Where(n => n.DesignationId == employeeDesignationId && !n.IsRead)
                .ToListAsync();
            foreach (var entity in entities)
            {
                entity.IsRead = true;
                entity.ReadDate = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();
            return true;
        }
        // Notifications belong to a designation, so each employee is mapped to the designation he or she holds now.
        // An employee without a current designation gets no notification. The caller saves the changes.
        public async Task AddForEmployeesAsync(IEnumerable<Guid> employeeIds, string title, string message, string icon, Guid referenceId, string referenceType, string actionUrl, Guid createdBy)
        {
            var employeeIdList = employeeIds.ToList();
            var designationIds = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => employeeIdList.Contains(ed.EmployeeId) && ed.EffectiveTo == null)
                .Select(ed => ed.DesignationId)
                .Distinct()
                .ToListAsync();

            foreach (var designationId in designationIds)
            {
                await context.TblNotifications.AddAsync(new TblNotification
                {
                    Id = Guid.NewGuid(),
                    DesignationId = designationId,
                    Title = title,
                    Icon = icon,
                    Message = message,
                    NotificationType = "info",
                    ReferenceId = referenceId,
                    ReferenceType = referenceType,
                    ActionUrl = actionUrl,
                    IsRead = false,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now
                });
            }
        }

        public async Task<int> GetCountAsync(Guid employeeId)
        {
            var employeeDesignationId = context.TblEmployeeDesignations
                .Where(ed => ed.EmployeeId == employeeId)
                .OrderByDescending(ed => ed.EffectiveFrom)
                .FirstOrDefault()?.DesignationId;
            if (employeeDesignationId == null || employeeDesignationId == Guid.Empty)
            {
                return 0;
            }
            var count = await context.TblNotifications.AsNoTracking()
                .Where(n => n.DesignationId == employeeDesignationId && !n.IsRead)
                .CountAsync();
            return count;
        }
    }
}
