using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface ISurveyCommitteeRepository
    {
        Task AddMember(SurveyCommittee surveyCommittee);
        Task UpdateMember(SurveyCommittee surveyCommittee);
        Task<SurveyCommittee?> GetByIdAsync(Guid id);
        Task<bool> ExistsByEmployeeIdAsync(Guid employeeId);
        Task<bool> IsActiveLeadAsync(Guid employeeId);
        Task<bool> AnyActiveMemberAsync(IEnumerable<Guid> employeeIds);
        Task<bool> IsSuperAdminAsync(Guid userId);
        Task<List<Guid>> GetActiveMemberEmployeeIdsAsync();
        Task<List<Guid>> GetRequiredEvaluatorEmployeeIdsAsync();
        Task<bool> IsActiveEvaluatorAsync(Guid employeeId);
        Task<List<Guid>> GetActiveLeadEmployeeIdsAsync();
    }
    public class SurveyCommitteeRepository(QubeFinDataContext context) : ISurveyCommitteeRepository
    {
        public async Task AddMember(SurveyCommittee surveyCommittee)
        {
            await context.TblSurveyCommittees.AddAsync(surveyCommittee.ToEntity());
        }
        public async Task UpdateMember(SurveyCommittee surveyCommittee)
        {
            context.TblSurveyCommittees.Update(surveyCommittee.ToEntity());
        }
        public async Task<SurveyCommittee?> GetByIdAsync(Guid id)
        {
            var entity = await context.TblSurveyCommittees.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return entity?.ToDomain();
        }
        // Only active members count, so a former member can be added again.
        public async Task<bool> ExistsByEmployeeIdAsync(Guid employeeId)
        {
            return await context.TblSurveyCommittees.AsNoTracking().AnyAsync(x => x.EmployeeId == employeeId && x.IsActive);
        }
        public async Task<bool> IsActiveLeadAsync(Guid employeeId)
        {
            return await context.TblSurveyCommittees.AsNoTracking().AnyAsync(x => x.EmployeeId == employeeId && x.IsActive && x.IsLead);
        }
        public async Task<bool> AnyActiveMemberAsync(IEnumerable<Guid> employeeIds)
        {
            var ids = employeeIds.ToList();
            return await context.TblSurveyCommittees.AsNoTracking().AnyAsync(x => x.IsActive && ids.Contains(x.EmployeeId));
        }
        public async Task<List<Guid>> GetActiveMemberEmployeeIdsAsync()
        {
            return await context.TblSurveyCommittees.AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => x.EmployeeId)
                .ToListAsync();
        }
        // The members who must evaluate: active members who are not Leads (the Lead gives the final decision).
        public async Task<List<Guid>> GetRequiredEvaluatorEmployeeIdsAsync()
        {
            return await context.TblSurveyCommittees.AsNoTracking()
                .Where(x => x.IsActive && !x.IsLead)
                .Select(x => x.EmployeeId)
                .ToListAsync();
        }
        public async Task<bool> IsActiveEvaluatorAsync(Guid employeeId)
        {
            return await context.TblSurveyCommittees.AsNoTracking().AnyAsync(x => x.EmployeeId == employeeId && x.IsActive && !x.IsLead);
        }
        public async Task<List<Guid>> GetActiveLeadEmployeeIdsAsync()
        {
            return await context.TblSurveyCommittees.AsNoTracking()
                .Where(x => x.IsActive && x.IsLead)
                .Select(x => x.EmployeeId)
                .ToListAsync();
        }
        public async Task<bool> IsSuperAdminAsync(Guid userId)
        {
            return await context.TblUsers.AsNoTracking().AnyAsync(x => x.Id == userId && x.IsSuperAdmin);
        }
    }
}
