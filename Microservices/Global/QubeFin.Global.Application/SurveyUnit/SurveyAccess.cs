using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit
{
    // Who is allowed to see a survey (its form and its photos).
    public static class SurveyAccess
    {
        // The creator, assigned surveyors, active committee members, and the MD once the survey has reached the MD.
        // A Group Survey is also seen by the people above its branch (see CanViewGroupSurveyAsync).
        // The survey must be loaded with its TblSurveyAssigneds.
        public static async Task<bool> CanViewAsync(QubeFinDataContext context, TblSurvey survey, Guid userId, Guid employeeId, CancellationToken cancellationToken)
        {
            if (survey.CreatedBy == userId) return true;
            if (survey.TblSurveyAssigneds.Any(a => a.EmployeeId == employeeId)) return true;

            if (survey.SurveyType == SurveyTypes.Group)
                return await CanViewGroupSurveyAsync(context, survey, employeeId, cancellationToken);

            var isCommitteeMember = await context.TblSurveyCommittees
                .AnyAsync(c => c.EmployeeId == employeeId && c.IsActive, cancellationToken);
            if (isCommitteeMember) return true;

            var reachedMd = survey.Status == SurveyStatuses.PendingApproval || survey.Status == SurveyStatuses.Approved
                || survey.Status == SurveyStatuses.Rejected || survey.Status == SurveyStatuses.BranchCreated;
            if (!reachedMd) return false;

            return await context.TblEmployeeDesignations.AnyAsync(ed =>
                ed.EmployeeId == employeeId
                && ed.EffectiveTo == null
                && ed.Designation.IsActive
                && ed.Designation.Post.Name == "Managing Director", cancellationToken);
        }

        // Group Survey: the person in charge of the branch, its Branch Manager, and the Area Manager and
        // Divisional Manager above the branch in the organization hierarchy.
        private static async Task<bool> CanViewGroupSurveyAsync(QubeFinDataContext context, TblSurvey survey, Guid employeeId, CancellationToken cancellationToken)
        {
            if (survey.BranchOrganizationUnitId is null) return false;
            var branchId = survey.BranchOrganizationUnitId.Value;

            // The approver (head of the unit above the branch) may not hold the Area Manager post, so he is checked directly.
            if (await BranchHierarchy.IsApproverAsync(context, employeeId, branchId)) return true;

            // The branch and every unit above it (parent, grandparent ...).
            var chain = new List<Guid>();
            Guid? current = branchId;
            while (current.HasValue && !chain.Contains(current.Value))
            {
                chain.Add(current.Value);
                current = await context.TblOrganizationUnits.AsNoTracking()
                    .Where(o => o.Id == current.Value)
                    .Select(o => o.ParentId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return await context.TblEmployeeDesignations.AnyAsync(ed =>
                ed.EmployeeId == employeeId
                && ed.EffectiveTo == null
                && ed.Designation.IsActive
                && (
                    (ed.Designation.OrganizationUnitId == branchId
                        && (ed.Designation.IsIncharge || ed.Designation.Post.Name == "Branch Manager"))
                    || (chain.Contains(ed.Designation.OrganizationUnitId)
                        && (ed.Designation.Post.Name == "Area Manager" || ed.Designation.Post.Name == "Divisional Manager"))
                ), cancellationToken);
        }
    }
}
