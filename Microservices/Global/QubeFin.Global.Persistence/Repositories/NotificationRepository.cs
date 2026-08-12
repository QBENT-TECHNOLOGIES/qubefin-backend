using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllUnreadAsync(Guid employeeId);
    }
    public class NotificationRepository(QubeFinDataContext context) : INotificationRepository
    {
        public async Task<IEnumerable<Notification>> GetAllUnreadAsync(Guid employeeId)
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
                .Where(n => n.DesignationId == employeeDesignationId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
            return entities.Select(m => m.ToDomain());
        }
    }
}
