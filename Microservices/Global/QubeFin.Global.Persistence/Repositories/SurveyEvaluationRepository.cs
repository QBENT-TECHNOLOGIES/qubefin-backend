using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface ISurveyEvaluationRepository
    {
        Task<List<TblSurveyCommitteeEvaluation>> GetActiveBySurveyIdAsync(Guid surveyId);
        Task<TblSurveyCommitteeEvaluation?> GetMemberEvaluationAsync(Guid surveyId, Guid employeeId);
        Task<TblSurveyCommitteeEvaluation?> GetLatestArchivedRecheckAsync(Guid surveyId);
        Task AddAsync(TblSurveyCommitteeEvaluation evaluation);
        Task ArchiveActiveAsync(Guid surveyId);
    }
    // "Active" means not archived. Evaluations are archived (kept as history) when the Lead asks for a Recheck.
    public class SurveyEvaluationRepository(QubeFinDataContext context) : ISurveyEvaluationRepository
    {
        public async Task<List<TblSurveyCommitteeEvaluation>> GetActiveBySurveyIdAsync(Guid surveyId)
        {
            return await context.TblSurveyCommitteeEvaluations.AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.SurveyId == surveyId && !x.IsArchived)
                .OrderBy(x => x.EvaluationDate)
                .ToListAsync();
        }
        // The member's own evaluation that can still be changed (tracked).
        public async Task<TblSurveyCommitteeEvaluation?> GetMemberEvaluationAsync(Guid surveyId, Guid employeeId)
        {
            return await context.TblSurveyCommitteeEvaluations
                .FirstOrDefaultAsync(x => x.SurveyId == surveyId && x.EmployeeId == employeeId && !x.IsArchived && !x.IsFinal);
        }
        // The Lead's last Recheck comment, shown to the surveyors.
        public async Task<TblSurveyCommitteeEvaluation?> GetLatestArchivedRecheckAsync(Guid surveyId)
        {
            return await context.TblSurveyCommitteeEvaluations.AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.SurveyId == surveyId && x.IsFinal && x.IsArchived && x.Status == EvaluationDecisions.Recheck)
                .OrderByDescending(x => x.EvaluationDate)
                .FirstOrDefaultAsync();
        }
        public async Task AddAsync(TblSurveyCommitteeEvaluation evaluation)
        {
            await context.TblSurveyCommitteeEvaluations.AddAsync(evaluation);
        }
        public async Task ArchiveActiveAsync(Guid surveyId)
        {
            var activeEvaluations = await context.TblSurveyCommitteeEvaluations
                .Where(x => x.SurveyId == surveyId && !x.IsArchived)
                .ToListAsync();

            foreach (var evaluation in activeEvaluations)
            {
                evaluation.IsArchived = true;
                evaluation.ArchivedOn = DateTime.Now;
            }
        }
    }
}
