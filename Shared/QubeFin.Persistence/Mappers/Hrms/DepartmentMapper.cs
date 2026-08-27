using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblDepartment;

namespace QubeFin.Persistence.Mappers.Hrms;


public static class DepartmentMapper
{
    public static Department ToDomain(this Entity entity)
    {
        return new Department(
            entity.Id,
            entity.Name,
            entity.IsActive,
            entity.CreatedOn,
            entity.CreatedBy,
            entity.LastModifiedOn,
            entity.LastModifiedBy);
    }
    public static Entity ToEntity(this Department domain)
    {
        return new Entity
        {
            Id = domain.Id,
            Name = domain.Name,
            IsActive = domain.IsActive,
            CreatedOn = domain.CreatedOn,
            CreatedBy = domain.CreatedBy,
            LastModifiedOn = domain.LastModifiedOn,
            LastModifiedBy = domain.LastModifiedBy
        };
    }
}
