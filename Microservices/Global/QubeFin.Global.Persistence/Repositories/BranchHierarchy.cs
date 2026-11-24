using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Persistence.Repositories
{
    // Who stands above a branch. Uses the same idea as the team hierarchy (Hrms: employees/hierarchy):
    // the head of an organization unit is its person in charge; if nobody is in charge, the holders of the head posts.
    public static class BranchHierarchy
    {
        private static readonly string[] HeadPosts = ["Managing Director", "Divisional Manager", "Area Manager", "Branch Manager"];
        private const int MaxDepth = 50;

        // The approver of a Group Survey: the head of the unit above the branch.
        // If that unit has no head, the next unit above that has one (several people can share the head).
        public static async Task<List<Guid>> GetApproverEmployeeIdsAsync(QubeFinDataContext context, Guid branchId)
        {
            var parents = await context.TblOrganizationUnits.AsNoTracking()
                .Select(u => new { u.Id, u.ParentId })
                .ToDictionaryAsync(u => u.Id, u => u.ParentId);

            var visited = new HashSet<Guid> { branchId };
            var unitId = parents.TryGetValue(branchId, out var firstParent) ? firstParent : null;

            for (var depth = 0; unitId.HasValue && visited.Add(unitId.Value) && depth < MaxDepth; depth++)
            {
                var heads = await GetUnitHeadEmployeeIdsAsync(context, unitId.Value);
                if (heads.Count > 0)
                    return heads;

                unitId = parents.TryGetValue(unitId.Value, out var parent) ? parent : null;
            }
            return new List<Guid>();
        }

        public static async Task<bool> IsApproverAsync(QubeFinDataContext context, Guid employeeId, Guid branchId)
        {
            return (await GetApproverEmployeeIdsAsync(context, branchId)).Contains(employeeId);
        }

        // The head of one unit: its person in charge; without one, everyone holding a head post there.
        private static async Task<List<Guid>> GetUnitHeadEmployeeIdsAsync(QubeFinDataContext context, Guid unitId)
        {
            var positions = await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null
                    && ed.Designation.IsActive
                    && ed.Employee.IsActive
                    && ed.Designation.OrganizationUnitId == unitId)
                .Select(ed => new { ed.EmployeeId, ed.Designation.IsIncharge, PostName = ed.Designation.Post.Name, ed.EffectiveFrom })
                .ToListAsync();

            var inCharge = positions.Where(p => p.IsIncharge).OrderBy(p => p.EffectiveFrom).Take(1).Select(p => p.EmployeeId).ToList();
            if (inCharge.Count > 0)
                return inCharge;

            return positions.Where(p => HeadPosts.Contains(p.PostName)).Select(p => p.EmployeeId).Distinct().ToList();
        }
    }
}
