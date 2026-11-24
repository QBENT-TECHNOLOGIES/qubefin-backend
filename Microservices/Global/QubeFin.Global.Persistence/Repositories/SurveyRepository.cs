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
        Task<string> GenerateAssignmentNoAsync(string surveyType, int year);
        Task<bool> IsAssignedToOpenSurveyAsync(Guid employeeId);
        Task UpdateStatusAsync(Guid surveyId, string status, Guid modifiedBy);
        Task<Guid?> GetCreatorEmployeeIdAsync(Guid surveyId);
        Task<BranchInfo?> GetHeadBranchAsync(Guid employeeId);
        Task<bool> IsHeadOfBranchAsync(Guid employeeId, Guid branchId);
        Task<List<Guid>> GetHeadEmployeeIdsOfBranchAsync(Guid branchId);
        Task<List<Guid>> GetApproverEmployeeIdsOfBranchAsync(Guid branchId);
        Task<bool> IsApproverOfBranchAsync(Guid employeeId, Guid branchId);
        Task<List<CreditOfficerInfo>> GetCreditOfficersOfBranchAsync(Guid branchId);
        Task SetBranchCreatedAsync(Guid surveyId, Guid branchOrganizationUnitId, Guid modifiedBy);
    }
    public record BranchInfo(Guid Id, string Name);
    public record CreditOfficerInfo(Guid EmployeeId, string EmployeeName);
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
            entity.AssignmentDate = survey.AssignmentDate;
            entity.ProposedArea = survey.ProposedArea;
            entity.AdministrativeUnitId = survey.AdministrativeUnitId;
            entity.TentativeSubmissionDate = survey.TentativeSubmissionDate;
            entity.SurveyFormAttached = survey.SurveyFormAttached;
            entity.BranchOrganizationUnitId = survey.BranchOrganizationUnitId;
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
                    // Existing assignment: AssignedBy and AssignedDate stay as they were
                    existing.EmployeeId = assignment.EmployeeId;
                    existing.IsLead = assignment.IsLead;
                }
            }
        }

        public async Task<Survey?> GetByIdAsync(Guid id)
        {
            var entity = await context.TblSurveys.Include(m => m.TblSurveyAssigneds).AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return entity?.ToDomain();
        }

        // Format: BS-2026-0001 (Branch Survey) or GS-2026-0001 (Group Survey). The number restarts every year.
        public async Task<string> GenerateAssignmentNoAsync(string surveyType, int year)
        {
            string typeCode = surveyType == SurveyTypes.Branch ? "BS" : "GS";
            string prefix = $"{typeCode}-{year}-";

            // The running number is zero-padded to 4 digits, so the highest text is also the highest number.
            var lastAssignmentNo = await context.TblSurveys
                .Where(s => s.AssignmentNo.StartsWith(prefix))
                .OrderByDescending(s => s.AssignmentNo)
                .Select(s => s.AssignmentNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastAssignmentNo != null && int.TryParse(lastAssignmentNo.Substring(prefix.Length), out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
            return $"{prefix}{nextNumber:D4}";
        }

        public async Task UpdateStatusAsync(Guid surveyId, string status, Guid modifiedBy)
        {
            var entity = await context.TblSurveys.FirstOrDefaultAsync(x => x.Id == surveyId);
            if (entity == null)
                throw new Exception("Survey not found.");

            entity.Status = status;
            entity.LastModifiedBy = modifiedBy;
            entity.LastModifiedOn = DateTime.Now;
        }

        // The employee behind the user who created the survey (null if that user has no employee).
        public async Task<Guid?> GetCreatorEmployeeIdAsync(Guid surveyId)
        {
            return await context.TblSurveys.AsNoTracking()
                .Where(s => s.Id == surveyId)
                .Join(context.TblUsers, s => s.CreatedBy, u => u.Id, (s, u) => u.EmployeeId)
                .FirstOrDefaultAsync();
        }

        // The branch was created from this survey: link it and finish the survey.
        public async Task SetBranchCreatedAsync(Guid surveyId, Guid branchOrganizationUnitId, Guid modifiedBy)
        {
            var entity = await context.TblSurveys.FirstOrDefaultAsync(x => x.Id == surveyId);
            if (entity == null)
                throw new Exception("Survey not found.");

            entity.BranchOrganizationUnitId = branchOrganizationUnitId;
            entity.Status = SurveyStatuses.BranchCreated;
            entity.LastModifiedBy = modifiedBy;
            entity.LastModifiedOn = DateTime.Now;
        }

        // The Head of Branch: the person in charge of the branch (current, active designation with IsIncharge = true).
        // If nobody is in charge, the Branch Manager of the branch is the head. The head creates the Group Survey and recommends it.
        private IQueryable<TblEmployeeDesignation> HeadDesignations(Guid employeeId)
        {
            return context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EmployeeId == employeeId
                    && ed.EffectiveTo == null
                    && ed.Designation.IsActive
                    && ed.Designation.OrganizationUnit.OrganizationUnitType.Name == "Branch"
                    && (ed.Designation.IsIncharge
                        || (ed.Designation.Post.Name == "Branch Manager"
                            && !context.TblEmployeeDesignations.Any(x => x.EffectiveTo == null
                                && x.Designation.IsActive
                                && x.Designation.IsIncharge
                                && x.Designation.OrganizationUnitId == ed.Designation.OrganizationUnitId))));
        }

        public async Task<BranchInfo?> GetHeadBranchAsync(Guid employeeId)
        {
            return await HeadDesignations(employeeId)
                .Select(ed => new BranchInfo(ed.Designation.OrganizationUnitId, ed.Designation.OrganizationUnit.Name))
                .FirstOrDefaultAsync();
        }

        // Everyone who is Head of this branch (normally one person).
        public async Task<List<Guid>> GetHeadEmployeeIdsOfBranchAsync(Guid branchId)
        {
            var inCharge = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null && ed.Designation.IsActive && ed.Designation.IsIncharge
                    && ed.Designation.OrganizationUnitId == branchId)
                .Select(ed => ed.EmployeeId).Distinct().ToListAsync();
            if (inCharge.Count > 0)
                return inCharge;

            return await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null && ed.Designation.IsActive && ed.Designation.Post.Name == "Branch Manager"
                    && ed.Designation.OrganizationUnitId == branchId)
                .Select(ed => ed.EmployeeId).Distinct().ToListAsync();
        }

        // The Area Manager side: the head of the unit above the branch (see BranchHierarchy).
        public Task<List<Guid>> GetApproverEmployeeIdsOfBranchAsync(Guid branchId)
        {
            return BranchHierarchy.GetApproverEmployeeIdsAsync(context, branchId);
        }

        public Task<bool> IsApproverOfBranchAsync(Guid employeeId, Guid branchId)
        {
            return BranchHierarchy.IsApproverAsync(context, employeeId, branchId);
        }

        public async Task<bool> IsHeadOfBranchAsync(Guid employeeId, Guid branchId)
        {
            return await HeadDesignations(employeeId).AnyAsync(ed => ed.Designation.OrganizationUnitId == branchId);
        }

        // Employees whose current designation has the post "Credit Officer" at this branch.
        public async Task<List<CreditOfficerInfo>> GetCreditOfficersOfBranchAsync(Guid branchId)
        {
            var rows = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null
                    && ed.Designation.IsActive
                    && ed.Designation.OrganizationUnitId == branchId
                    && ed.Designation.Post.Name == "Credit Officer")
                .Select(ed => new { ed.EmployeeId, ed.Employee.FullName, ed.Employee.Code })
                .ToListAsync();

            // Distinct and sorting are done here, because EF cannot translate them on the built name.
            return rows
                .GroupBy(r => r.EmployeeId)
                .Select(g => new CreditOfficerInfo(g.Key, g.First().FullName + " (" + g.First().Code + ")"))
                .OrderBy(x => x.EmployeeName)
                .ToList();
        }

        // "Open" means the survey is not finished yet (not Approved, Rejected or Branch Created).
        public async Task<bool> IsAssignedToOpenSurveyAsync(Guid employeeId)
        {
            var closedStatuses = new[] { SurveyStatuses.Approved, SurveyStatuses.Rejected, SurveyStatuses.BranchCreated };
            return await context.TblSurveyAssigneds.AsNoTracking()
                .AnyAsync(a => a.EmployeeId == employeeId && !closedStatuses.Contains(a.Survey.Status));
        }
    }
}
