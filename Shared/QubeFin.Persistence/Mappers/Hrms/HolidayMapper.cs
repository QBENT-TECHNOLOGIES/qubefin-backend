using QubeFin.Persistence.Models.Hrms;
using Entity = QubeFin.Persistence.Entities.TblHoliday;

namespace QubeFin.Persistence.Mappers.Hrms;

public static class HolidayMapper
{
    public static Holiday ToDomain(this Entity entity)
    {
        return new Holiday(
            entity.Id,
            entity.OrgUnitId,
            entity.HolidayDate,
            entity.Description,
            entity.CreatedOn,
            entity.CreatedBy,
            entity.LastModifiedOn,
            entity.LastModifiedBy);
    }

    public static Entity ToEntity(this Holiday domain)
    {
        return new Entity
        {
            Id = domain.Id,
            OrgUnitId = domain.OrgUnitId,
            HolidayDate = domain.HolidayDate,
            Description = domain.Description,
            CreatedOn = domain.CreatedOn,
            CreatedBy = domain.CreatedBy,
            LastModifiedOn = domain.LastModifiedOn,
            LastModifiedBy = domain.LastModifiedBy
        };
    }
}
