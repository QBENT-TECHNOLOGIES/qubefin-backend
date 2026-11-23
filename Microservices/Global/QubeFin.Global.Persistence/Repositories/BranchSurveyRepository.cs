using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface IBranchSurveyRepository
    {
        Task<TblBranchSurvey?> GetBySurveyIdAsync(Guid surveyId);
        Task AddAsync(TblBranchSurvey branchSurvey);
        void ReplaceCompetitors(TblBranchSurvey branchSurvey, List<TblBranchSurveyCompetitor> newCompetitors);
    }
    // The branch survey form has about 100 plain fields, so this repository works with the entity directly.
    public class BranchSurveyRepository(QubeFinDataContext context) : IBranchSurveyRepository
    {
        public async Task<TblBranchSurvey?> GetBySurveyIdAsync(Guid surveyId)
        {
            return await context.TblBranchSurveys
                .Include(x => x.TblBranchSurveyCompetitors)
                .FirstOrDefaultAsync(x => x.SurveyId == surveyId);
        }
        public async Task AddAsync(TblBranchSurvey branchSurvey)
        {
            await context.TblBranchSurveys.AddAsync(branchSurvey);
        }
        // Deletes the old competitor rows and adds the new ones.
        public void ReplaceCompetitors(TblBranchSurvey branchSurvey, List<TblBranchSurveyCompetitor> newCompetitors)
        {
            context.TblBranchSurveyCompetitors.RemoveRange(branchSurvey.TblBranchSurveyCompetitors.ToList());
            foreach (var competitor in newCompetitors)
            {
                context.TblBranchSurveyCompetitors.Add(competitor);
            }
        }
    }
}
