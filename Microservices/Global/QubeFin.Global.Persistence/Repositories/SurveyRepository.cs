using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

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
            var entity = await context.TblSurveys
                .Include(x => x.TblSurveyAssigneds)
                .FirstOrDefaultAsync(x => x.Id == survey.Id);

            if (entity == null)
                throw new Exception("Survey not found.");

            // Update parent
            entity.SurveyType = survey.SurveyType;
            entity.AssignmentDate = survey.AssignmentDate;
            entity.ProposedArea = survey.ProposedArea;
            entity.AdministrativeUnitId = survey.AdministrativeUnitId;
            entity.TentativeSubmissionDate = survey.TentativeSubmissionDate;
            entity.LastModifiedBy = survey.LastModifiedBy;
            entity.LastModifiedOn = survey.LastModifiedOn;

            // Remove deleted assignments
            foreach (var existing in entity.TblSurveyAssigneds.ToList())
            {
                if (!survey.SurveyAssigneds.Any(x => x.Id == existing.Id))
                {
                    context.TblSurveyAssigneds.Remove(existing);
                }
            }

            foreach (var assignment in survey.SurveyAssigneds)
            {
                var existing = entity.TblSurveyAssigneds.FirstOrDefault(x => x.Id == assignment.Id);

                if (existing == null)
                {
                    // New assignment
                    var newSurveyAssigned = SurveyAssigned.Create(entity.Id, assignment.EmployeeId, assignment.IsLead, assignment.AssignedBy);
                    entity.TblSurveyAssigneds.Add(newSurveyAssigned.ToEntity());
                }
                else
                {
                    // Existing assignment
                    existing.EmployeeId = assignment.EmployeeId;
                    existing.IsLead = assignment.IsLead;
                    existing.AssignedBy = assignment.AssignedBy;
                    existing.AssignedDate = assignment.AssignedDate;
                }
            }
        }

        public async Task<Survey?> GetByIdAsync(Guid id)
        {
            var entity = await context.TblSurveys.Include(m => m.TblSurveyAssigneds).AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return entity?.ToDomain();
        }
    }
}
