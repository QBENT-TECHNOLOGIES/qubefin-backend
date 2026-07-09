using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Payrolls;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Persistence.Repositories
{   
    public interface IPayrollRepository
    {
        Task CreatePayroll(PayrollModel payroll);
        Task UpdatePayroll(PayrollModel payroll);
        Task<PayrollModel?> GetPayrollById(Guid payrollId);
        Task<IEnumerable<PayrollModel>> GetAllPayrolls();
    }
    public class PayrollRepository(QubeFinDataContext context) : IPayrollRepository
    {
        public async Task CreatePayroll(PayrollModel payroll)
        {
            await context.TblPayRolls.AddAsync(payroll.ToEntity());
        }
        public Task UpdatePayroll(PayrollModel payroll)
        {
            context.TblPayRolls.Update(payroll.ToEntity());
            return Task.CompletedTask;
        }
        public async Task<PayrollModel?> GetPayrollById(Guid payrollId)
        {
            var entity = await context.TblPayRolls.Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company).Include(m => m.FinYear).AsNoTracking().FirstOrDefaultAsync(x => x.Id == payrollId);
            return entity?.ToDomain();
        }
        public async Task<IEnumerable<PayrollModel>> GetAllPayrolls()
        {
            var entities = await context.TblPayRolls.Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company).Include(m => m.FinYear).AsNoTracking().ToListAsync();
            return entities.Select(e => e.ToDomain());
        }
    }
}
