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
        public async Task<bool> ExistsByEmployeeIdAsync(Guid employeeId)
        {
            return await context.TblSurveyCommittees.AsNoTracking().AnyAsync(x => x.EmployeeId == employeeId);
        }
    }
}
