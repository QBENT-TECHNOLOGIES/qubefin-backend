using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Payrolls;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Persistence.Repositories
{   
    public interface IPayrollRepository
    {
        Task<PayrollModel?> GetPayrollById(Guid payrollId);
        Task<IEnumerable<PayrollModel>> GetAllPayrolls();
        Task<MonthlyPayroll> GetMonthlyPayrollAsync(int payrollMonth, int payrollYear);
        Task<List<TblPayRoll>> GetPayrollsForUpdateAsync(int month, int year, CancellationToken cancellationToken = default);
        Task<IEnumerable<MonthwisePayrollData>> GetMonthwisePayrollSummaryAsync();
        Task<bool> HasOpenPayrollAsync(Guid companyId, CancellationToken cancellationToken);
        Task CreatePayrollAsync(Guid companyId, Guid? userId, CancellationToken cancellationToken);
        Task<bool> IsPayrollAvailableForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdatePayrollComponentsAsync(Guid payrollId, List<PayrollComponentModel> updatedComponents, CancellationToken cancellationToken = default);
    }
    public class PayrollRepository(QubeFinDataContext context) : IPayrollRepository
    {
        public async Task<PayrollModel?> GetPayrollById(Guid payrollId)
        {
            var entity = await context.TblPayRolls.Include(m => m.OrganizationUnit).Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company).Include(m => m.FinYear)
                .Include(m => m.TblPayRollComponents)
                    .ThenInclude(c => c.SalaryComponent)
                        .ThenInclude(sc => sc.Category)
                        .Include(m => m.SalaryGrade).AsNoTracking().FirstOrDefaultAsync(x => x.Id == payrollId);
            return entity?.ToDomain();
        }
        public async Task<IEnumerable<PayrollModel>> GetAllPayrolls()
        {
            var entities = await context.TblPayRolls.Include(m => m.OrganizationUnit).Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company).Include(m => m.FinYear).AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
        public async Task<MonthlyPayroll> GetMonthlyPayrollAsync(int payrollMonth, int payrollYear)
        {
            var entities = await context.TblPayRolls
                .Include(x => x.Employee)
                .Include(x => x.OrganizationUnit)
                .Include(x => x.Designation)
                .Include(x => x.Company)
                .Include(x => x.FinYear)
                .Include(x => x.TblPayRollComponents)
                    .ThenInclude(x => x.SalaryComponent)
                        .ThenInclude(x => x.Category)
                .Include(m => m.SalaryGrade)
                .AsNoTracking()
                .Where(x => x.PayrollMonth == payrollMonth && x.PayrollYear == payrollYear)
                .ToListAsync();

            var payrollModels = entities.Select(e => e.ToDomain());
            return payrollModels.ToMonthlyPayroll(payrollMonth, payrollYear);
        }
        public async Task<bool> HasOpenPayrollAsync(Guid companyId, CancellationToken cancellationToken)
        {
            return await context.TblPayRolls .AnyAsync(p => p.CompanyId == companyId && p.IsLocked == false, cancellationToken);
        }
        public async Task<IEnumerable<MonthwisePayrollData>> GetMonthwisePayrollSummaryAsync()
        {
            var sql = "EXEC Payroll.USP_GetMonthlyPayroll";
            var result = await context.Database
                .SqlQueryRaw<MonthwisePayrollData>(sql)
                .ToListAsync();

            return result;
        }
        public async Task<List<TblPayRoll>> GetPayrollsForUpdateAsync(int month, int year, CancellationToken cancellationToken = default)
        {
            return await context.TblPayRolls
                .Where(x => x.PayrollMonth == month && x.PayrollYear == year)
                .ToListAsync(cancellationToken);
        }
        public async Task CreatePayrollAsync(Guid companyId, Guid? userId, CancellationToken cancellationToken)
        {
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC Payroll.USP_CreatePayroll @CompanyId = {companyId}, @UserId = {userId}",
                cancellationToken);
        }
        public async Task<bool> IsPayrollAvailableForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.TblPayRolls
                .AsNoTracking()
                .AnyAsync(m => m.Id == id && !m.IsLocked, cancellationToken);
        }
        public async Task<bool> UpdatePayrollComponentsAsync(Guid payrollId, List<PayrollComponentModel> updatedComponents, CancellationToken cancellationToken = default)
        {
            var payroll = await context.TblPayRolls
                .Include(p => p.TblPayRollComponents)
                .FirstOrDefaultAsync(p => p.Id == payrollId, cancellationToken);

            if (payroll == null)
            {
                return false;
            }

            if (payroll.TblPayRollComponents != null && payroll.TblPayRollComponents.Any())
            {
                foreach (var updateItem in updatedComponents)
                {
                    var existingComponent = payroll.TblPayRollComponents
                        .FirstOrDefault(c => c.SalaryComponentId == updateItem.SalaryComponentId);
                    if (existingComponent != null)
                    {
                        existingComponent.Amount = updateItem.Amount;
                    }
                }
            }
            return true;
        }
    }
}
