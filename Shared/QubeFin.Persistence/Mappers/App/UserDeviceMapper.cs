using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblUserDevice;

namespace QubeFin.Persistence.Mappers.App;

public static class UserDeviceMapper
{
    public static UserDevice ToDomain(this Entity entity)
    {
        return new UserDevice(
            entity.Id,
            entity.UserId,
            entity.DeviceId,
            entity.ReleaseDate,
            entity.IsReleased,
            entity.AssignDate,
            entity.ReleaseBy
            );
    }

    public static Entity ToEntity(this UserDevice domain)
    {
        return new Entity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            DeviceId = domain.DeviceId,
            ReleaseBy = domain.ReleaseBy,
            ReleaseDate = domain.ReleaseDate,
            AssignDate = domain.AssignDate,
            IsReleased = domain.IsReleased
        };
    }
}
