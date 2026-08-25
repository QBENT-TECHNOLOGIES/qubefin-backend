using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
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
