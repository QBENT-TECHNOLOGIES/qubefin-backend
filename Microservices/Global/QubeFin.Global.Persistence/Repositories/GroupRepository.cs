using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface IGroupRepository
    {
        Task<TblGroup?> GetByIdAsync(Guid id);
        Task AddAsync(TblGroup group);
        Task<BranchInfo?> GetBranchByCodeAsync(int branchCode);
        Task<int?> GetBranchCodeAsync(Guid branchId);
        Task<int> GetNextCodeValAsync(int branchCode);
        Task<bool> NameExistsAsync(int branchCode, string name, Guid? excludedId);
        Task<List<CreditOfficerPost>> GetCreditOfficerPostsOfBranchAsync(Guid branchId);
        Task<CreditOfficerPost?> GetCreditOfficerPostOfEmployeeAsync(Guid employeeId);
        Task<List<Guid>> GetCurrentHolderEmployeeIdsAsync(Guid designationId);
    }

    // A current Credit Officer position: the designation (which owns the groups) and who holds it now.
    public record CreditOfficerPost(Guid DesignationId, Guid EmployeeId, string EmployeeName, Guid BranchId, string BranchName);

    // Group Master data access. The branch of a group is found from Tbl_Group.BranchCode = the branch organization unit code.
    public class GroupRepository(QubeFinDataContext context) : IGroupRepository
    {
        public async Task<TblGroup?> GetByIdAsync(Guid id)
        {
            return await context.TblGroups.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task AddAsync(TblGroup group)
        {
            await context.TblGroups.AddAsync(group);
        }

        public async Task<BranchInfo?> GetBranchByCodeAsync(int branchCode)
        {
            return await context.TblOrganizationUnits.AsNoTracking()
                .Where(o => o.CodeVal == branchCode && o.OrganizationUnitType.Name == "Branch")
                .Select(o => new BranchInfo(o.Id, o.Name))
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetBranchCodeAsync(Guid branchId)
        {
            return await context.TblOrganizationUnits.AsNoTracking()
                .Where(o => o.Id == branchId)
                .Select(o => (int?)o.CodeVal)
                .FirstOrDefaultAsync();
        }

        // The sequence (1, 2, 3 ...) restarts for each branch.
        public async Task<int> GetNextCodeValAsync(int branchCode)
        {
            var highest = await context.TblGroups.AsNoTracking()
                .Where(g => g.BranchCode == branchCode)
                .MaxAsync(g => (int?)g.CodeVal);
            return (highest ?? 0) + 1;
        }

        public async Task<bool> NameExistsAsync(int branchCode, string name, Guid? excludedId)
        {
            var lowered = name.Trim().ToLower();
            return await context.TblGroups.AsNoTracking()
                .AnyAsync(g => g.BranchCode == branchCode
                    && g.Name.Trim().ToLower() == lowered
                    && (excludedId == null || g.Id != excludedId));
        }

        public async Task<List<CreditOfficerPost>> GetCreditOfficerPostsOfBranchAsync(Guid branchId)
        {
            return await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EffectiveTo == null
                    && ed.Designation.IsActive
                    && ed.Designation.OrganizationUnitId == branchId
                    && ed.Designation.Post.Name == "Credit Officer")
                .Select(ed => new CreditOfficerPost(ed.DesignationId, ed.EmployeeId, ed.Employee.FullName + " (" + ed.Employee.Code + ")",
                    ed.Designation.OrganizationUnitId, ed.Designation.OrganizationUnit.Name))
                .ToListAsync();
        }

        // The Credit Officer position the employee holds now, at a branch.
        public async Task<CreditOfficerPost?> GetCreditOfficerPostOfEmployeeAsync(Guid employeeId)
        {
            return await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.EmployeeId == employeeId
                    && ed.EffectiveTo == null
                    && ed.Designation.IsActive
                    && ed.Designation.Post.Name == "Credit Officer"
                    && ed.Designation.OrganizationUnit.OrganizationUnitType.Name == "Branch")
                .Select(ed => new CreditOfficerPost(ed.DesignationId, ed.EmployeeId, ed.Employee.FullName + " (" + ed.Employee.Code + ")",
                    ed.Designation.OrganizationUnitId, ed.Designation.OrganizationUnit.Name))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Guid>> GetCurrentHolderEmployeeIdsAsync(Guid designationId)
        {
            return await context.TblEmployeeDesignations.AsNoTracking()
                .Where(ed => ed.DesignationId == designationId && ed.EffectiveTo == null)
                .Select(ed => ed.EmployeeId)
                .Distinct()
                .ToListAsync();
        }
    }
}
