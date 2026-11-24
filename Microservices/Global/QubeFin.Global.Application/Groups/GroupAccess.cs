using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.Groups
{
    // The groups an employee can see: the groups of his own designation, and the groups of the branches
    // he heads (person in charge / Branch Manager), or is the Area Manager / Divisional Manager / approver of.
    public record GroupVisibility(List<Guid> OwnDesignationIds, HashSet<int> BranchCodes);

    public record GroupRights(BranchInfo? Branch, bool CanView, bool CanEdit, bool CanDecide);

    // Who is adding a group: the Head of a branch (any Credit Officer of the branch) or a Credit Officer (himself).
    public record GroupAdder(Guid BranchId, string BranchName, bool IsHead, List<CreditOfficerPost> CreditOfficers);

    public static class GroupAccess
    {
        public static async Task<GroupAdder?> GetAdderAsync(ISurveyRepository surveyRepository, IGroupRepository groupRepository, Guid employeeId)
        {
            var headBranch = await surveyRepository.GetHeadBranchAsync(employeeId);
            if (headBranch is not null)
                return new GroupAdder(headBranch.Id, headBranch.Name, true, await groupRepository.GetCreditOfficerPostsOfBranchAsync(headBranch.Id));

            var post = await groupRepository.GetCreditOfficerPostOfEmployeeAsync(employeeId);
            return post is null ? null : new GroupAdder(post.BranchId, post.BranchName, false, [post]);
        }

        public static async Task<GroupVisibility> GetVisibilityAsync(QubeFinDataContext context, Guid employeeId, CancellationToken cancellationToken)
        {
            var mine = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EmployeeId == employeeId && ed.EffectiveTo == null && ed.Designation.IsActive)
                .Select(ed => new { ed.DesignationId, ed.Designation.OrganizationUnitId, ed.Designation.IsIncharge, PostName = ed.Designation.Post.Name })
                .ToListAsync(cancellationToken);

            var ownDesignationIds = mine.Select(m => m.DesignationId).ToList();
            var codes = new HashSet<int>();
            if (mine.Count == 0)
                return new GroupVisibility(ownDesignationIds, codes);

            var units = await context.TblOrganizationUnits.AsNoTracking()
                .Select(o => new { o.Id, o.ParentId, o.CodeVal, TypeName = o.OrganizationUnitType.Name })
                .ToListAsync(cancellationToken);
            var parents = units.ToDictionary(u => u.Id, u => u.ParentId);
            var myUnitIds = mine.Select(m => m.OrganizationUnitId).ToHashSet();

            foreach (var branch in units.Where(u => u.TypeName == "Branch"))
            {
                var visible = false;
                var mayBeApprover = false;
                Guid? current = branch.Id;
                var seen = new HashSet<Guid>();
                while (current.HasValue && seen.Add(current.Value))
                {
                    if (myUnitIds.Contains(current.Value))
                    {
                        mayBeApprover = true;
                        foreach (var m in mine.Where(m => m.OrganizationUnitId == current.Value))
                        {
                            if (current.Value == branch.Id && (m.IsIncharge || m.PostName == "Branch Manager")) visible = true;
                            if (current.Value != branch.Id && (m.PostName == "Area Manager" || m.PostName == "Divisional Manager")) visible = true;
                        }
                    }
                    current = parents.TryGetValue(current.Value, out var parent) ? parent : null;
                }

                // The approver is the head of the first unit above the branch that has a head; only people who hold
                // a position at or above the branch can be that, so the exact check runs for them only.
                if (!visible && mayBeApprover && await BranchHierarchy.IsApproverAsync(context, employeeId, branch.Id))
                    visible = true;

                if (visible) codes.Add(branch.CodeVal);
            }

            return new GroupVisibility(ownDesignationIds, codes);
        }

        public static async Task<GroupRights> GetRightsAsync(
            QubeFinDataContext context,
            ISurveyRepository surveyRepository,
            IGroupRepository groupRepository,
            TblGroup group,
            Guid userId,
            Guid employeeId,
            CancellationToken cancellationToken)
        {
            var branch = group.BranchCode.HasValue ? await groupRepository.GetBranchByCodeAsync(group.BranchCode.Value) : null;

            var isOwner = group.DesignationId.HasValue && await context.TblEmployeeDesignations.AnyAsync(ed =>
                ed.EmployeeId == employeeId && ed.EffectiveTo == null && ed.DesignationId == group.DesignationId, cancellationToken);
            var isHead = branch is not null && await surveyRepository.IsHeadOfBranchAsync(employeeId, branch.Id);
            var isApprover = branch is not null && await surveyRepository.IsApproverOfBranchAsync(employeeId, branch.Id);

            var canView = isOwner || isHead || isApprover;
            if (!canView)
            {
                var visibility = await GetVisibilityAsync(context, employeeId, cancellationToken);
                canView = group.BranchCode.HasValue && visibility.BranchCodes.Contains(group.BranchCode.Value);
            }

            var status = (group.CurrentStatus ?? string.Empty).Trim();
            // Nobody approves a group he added himself.
            var canDecide = status == GroupStatuses.Pending && group.CreatedBy != userId && (isHead || isApprover);

            return new GroupRights(branch, canView, isOwner || isHead, canDecide);
        }
    }
}
