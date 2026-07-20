using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class SurveyMapper
    {
        public static Survey ToDomain(this TblSurvey entity)
        {
            if (entity == null) return null!;
            return new Survey(
                entity.Id,
                entity.SurveyType,
                entity.AssignmentNo,
                entity.AssignmentDate,
                entity.ProposedArea,
                entity.AdministrativeUnitId,
                entity.TentativeSubmissionDate,
                entity.CreatedBy,
                entity.CreatedOn,
                entity.LastModifiedBy,
                entity.LastModifiedOn
            );
        }
        public static TblSurvey ToEntity(this Survey domain)
        {
            if (domain == null) return null!;
            return new TblSurvey
            {
                Id = domain.Id,
                SurveyType = domain.SurveyType,
                AssignmentNo = domain.AssignmentNo,
                AssignmentDate = domain.AssignmentDate,
                ProposedArea = domain.ProposedArea,
                AdministrativeUnitId = domain.AdministrativeUnitId,
                TentativeSubmissionDate = domain.TentativeSubmissionDate,
                CreatedBy = domain.CreatedBy,
                CreatedOn = domain.CreatedOn,
                LastModifiedBy = domain.LastModifiedBy,
                LastModifiedOn = domain.LastModifiedOn
            };
        }
    }
}
