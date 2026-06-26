using QubeFin.Persistence.Models.App;
using Entity = QubeFin.Persistence.Entities.TblUser;

namespace QubeFin.Persistence.Mappers.App;

public static class UserMapper
{
    public static User ToDomain(this Entity entity)
    {
        return new User(
            entity.Id,
            entity.UserName,
            entity.Password,
            entity.EmployeeId,
            entity.IsActive,
            entity.IsSuperAdmin,
            entity.HasMfaEnabled,
            entity.MfaSecret,
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn);
    }

    public static Entity ToEntity(this User domain)
    {
        return new Entity
        {
            Id = domain.Id,
            UserName = domain.UserName,
            Password = domain.Password,
            EmployeeId = domain.EmployeeId,
            HasMfaEnabled = domain.HasMfaEnabled,
            MfaSecret = domain.MfaSecret,
            IsActive = domain.IsActive,
            IsSuperAdmin = domain.IsSuperAdmin,
            CreatedBy = domain.CreatedBy,
            CreatedOn = domain.CreatedOn,
            LastModifiedBy = domain.LastModifiedBy,
            LastModifiedOn = domain.LastModifiedOn
        };
    }
}
