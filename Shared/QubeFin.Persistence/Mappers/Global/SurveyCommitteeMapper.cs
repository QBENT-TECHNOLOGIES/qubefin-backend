using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblSurveyCommittee;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class SurveyCommitteeMapper
    {
        public static SurveyCommittee ToDomain(this Entity entity)
        {
            var domain = new SurveyCommittee(
                entity.Id,
                entity.EmployeeId,
                entity.IsLead,
                entity.IsActive,
                entity.AssignedFrom,
                entity.AssignedTo,
                entity.CreatedBy,
                entity.CreatedOn,
                entity.LastModifiedBy,
                entity.LastModifiedOn
            );
            return domain;
        }
        public static Entity ToEntity(this SurveyCommittee domain)
        {
            return new Entity
            {
                Id = domain.Id,
                EmployeeId = domain.EmployeeId,
                IsLead = domain.IsLead,
                IsActive = domain.IsActive,
                AssignedFrom = domain.AssignedFrom,
                AssignedTo = domain.AssignedTo,
                CreatedOn = domain.CreatedOn,
                CreatedBy = domain.CreatedBy,
                LastModifiedOn = domain.LastModifiedOn,
                LastModifiedBy = domain.LastModifiedBy
            };
        }
    }
}
