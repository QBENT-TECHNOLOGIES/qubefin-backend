using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblNotification;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class NotificationMapper
    {
        public static Notification ToDomain(this Entity entity)
        {
            return new Notification
            (entity.Id, entity.DesignationId, entity.Title, entity.Message, entity.NotificationType, entity.ReferenceId, entity.ReferenceType, entity.ActionUrl, entity.IsRead, entity.ReadDate, entity.CreatedBy, entity.CreatedOn);

        }
    }
}