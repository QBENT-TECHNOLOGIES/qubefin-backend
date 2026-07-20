using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface ISurveyRepository
    {
        Task AddSurvey(Survey survey);
        Task UpdateSurvey(Survey survey);
        Task<Survey?> GetByIdAsync(Guid id);
    }
    public class SurveyRepository(QubeFinDataContext context) : ISurveyRepository
    {
        public async Task AddSurvey(Survey survey)
        {
            await context.TblSurveys.AddAsync(survey.ToEntity());
        }
        public async Task UpdateSurvey(Survey survey)
        {
            context.TblSurveys.Update(survey.ToEntity());
        }
        public async Task<Survey?> GetByIdAsync(Guid id)
        {
            var entity = await context.TblSurveys.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return entity?.ToDomain();
        }
    }
}
