using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface ISurveyDocumentRepository
    {
        Task AddAsync(TblSurveyDocument document);
        Task<TblSurveyDocument?> GetByIdAsync(Guid id);
        Task<List<TblSurveyDocument>> GetBySurveyIdAsync(Guid surveyId);
        Task<int> CountAsync(Guid surveyId);
        void Remove(TblSurveyDocument document);
    }
    public class SurveyDocumentRepository(QubeFinDataContext context) : ISurveyDocumentRepository
    {
        public async Task AddAsync(TblSurveyDocument document)
        {
            await context.TblSurveyDocuments.AddAsync(document);
        }
        public async Task<TblSurveyDocument?> GetByIdAsync(Guid id)
        {
            return await context.TblSurveyDocuments.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<TblSurveyDocument>> GetBySurveyIdAsync(Guid surveyId)
        {
            return await context.TblSurveyDocuments.AsNoTracking()
                .Where(x => x.SurveyId == surveyId)
                .OrderBy(x => x.Sequence)
                .ToListAsync();
        }
        public async Task<int> CountAsync(Guid surveyId)
        {
            return await context.TblSurveyDocuments.AsNoTracking().CountAsync(x => x.SurveyId == surveyId);
        }
        public void Remove(TblSurveyDocument document)
        {
            context.TblSurveyDocuments.Remove(document);
        }
    }
}
