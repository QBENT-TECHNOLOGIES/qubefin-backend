using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblUserSession;

namespace QubeFin.Persistence.Mappers.App;

public static class UserSessionMapper
{
    public static UserSession ToDomain(this Entity entity)
    {
        return new UserSession(
            entity.Id,
            entity.UserId,
            entity.SessionToken,
            entity.IsMfaVerified,
            entity.AccessToken,
            entity.RefreshToken,
            entity.LoginTime,
            entity.LogoutTime);
    }

    public static Entity ToEntity(this UserSession domain)
    {
        return new Entity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            SessionToken = domain.SessionToken,
            IsMfaVerified = domain.IsMfaVerified,
            AccessToken = domain.AccessToken,
            RefreshToken = domain.RefreshToken,
            LoginTime = domain.LoginTime,
            LogoutTime = domain.LogoutTime
        };
    }
}