using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface ISurveyApprovalRepository
    {
        // companyId: the Managing Director of that company (the post held at an organization unit of the company).
        // null (a survey from before companies): any holder of the post.
        Task<List<Guid>> GetManagingDirectorEmployeeIdsAsync(Guid? companyId);
        Task<bool> IsManagingDirectorAsync(Guid employeeId, Guid? companyId);
        Task<TblSurveyApproval?> GetLatestAsync(Guid surveyId);
        Task<TblSurveyApproval?> GetLatestRecheckAsync(Guid surveyId);
        Task AddAsync(TblSurveyApproval approval);
    }
    // The Managing Director is whoever currently holds the "Managing Director" post (several holders are allowed).
    public class SurveyApprovalRepository(QubeFinDataContext context) : ISurveyApprovalRepository
    {
        private const string ManagingDirectorPost = "Managing Director";

        public async Task<List<Guid>> GetManagingDirectorEmployeeIdsAsync(Guid? companyId)
        {
            return await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null && ed.Designation.IsActive && ed.Designation.Post.Name == ManagingDirectorPost
                    && (companyId == null || ed.Designation.OrganizationUnit.CompanyId == companyId))
                .Select(ed => ed.EmployeeId)
                .Distinct()
                .ToListAsync();
        }
        public async Task<bool> IsManagingDirectorAsync(Guid employeeId, Guid? companyId)
        {
            return await context.TblEmployeeDesignations.AsNoTracking()
                .AnyAsync(ed => ed.EmployeeId == employeeId && ed.EffectiveTo == null && ed.Designation.IsActive
                    && ed.Designation.Post.Name == ManagingDirectorPost
                    && (companyId == null || ed.Designation.OrganizationUnit.CompanyId == companyId));
        }
        public async Task<TblSurveyApproval?> GetLatestAsync(Guid surveyId)
        {
            return await context.TblSurveyApprovals.AsNoTracking()
                .Include(x => x.AuthorityEmployee)
                .Where(x => x.SurveyId == surveyId)
                .OrderByDescending(x => x.ActionOn)
                .FirstOrDefaultAsync();
        }
        public async Task<TblSurveyApproval?> GetLatestRecheckAsync(Guid surveyId)
        {
            return await context.TblSurveyApprovals.AsNoTracking()
                .Where(x => x.SurveyId == surveyId && x.Action == EvaluationDecisions.Recheck)
                .OrderByDescending(x => x.ActionOn)
                .FirstOrDefaultAsync();
        }
        public async Task AddAsync(TblSurveyApproval approval)
        {
            await context.TblSurveyApprovals.AddAsync(approval);
        }
    }
}
