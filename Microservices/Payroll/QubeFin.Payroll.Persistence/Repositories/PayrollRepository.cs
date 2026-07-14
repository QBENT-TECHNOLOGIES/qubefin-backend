using Microsoft.EntityFrameworkCore;
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
    }
    public class PayrollRepository(QubeFinDataContext context) : IPayrollRepository
    {
        public async Task<PayrollModel?> GetPayrollById(Guid payrollId)
        {
            var entity = await context.TblPayRolls.Include(m => m.OrganizationUnit).Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company).Include(m => m.FinYear).AsNoTracking().FirstOrDefaultAsync(x => x.Id == payrollId);
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
                .AsNoTracking()
                .Where(x => x.PayrollMonth == payrollMonth && x.PayrollYear == payrollYear)
                .ToListAsync();

            var payrollModels = entities.Select(e => e.ToDomain());
            return payrollModels.ToMonthlyPayroll(payrollMonth, payrollYear);
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
    }
}
