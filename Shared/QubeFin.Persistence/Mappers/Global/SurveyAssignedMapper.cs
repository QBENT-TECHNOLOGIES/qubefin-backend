using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class SurveyAssignedMapper
    {
        public static SurveyAssigned ToDomain(this TblSurveyAssigned entity)
        {
            if (entity == null) return null!;

            return new SurveyAssigned(
                entity.Id,
                entity.SurveyId,
                entity.EmployeeId,
                entity.IsLead,
                entity.AssignedBy
            );
        }

        public static TblSurveyAssigned ToEntity(this SurveyAssigned domain)
        {
            if (domain == null) return null!;

            return new TblSurveyAssigned
            {
                Id = domain.Id,
                SurveyId = domain.SurveyId,
                EmployeeId = domain.EmployeeId,
                IsLead = domain.IsLead,
                AssignedDate = domain.AssignedDate,
                AssignedBy = domain.AssignedBy
            };
        }
    }

}
