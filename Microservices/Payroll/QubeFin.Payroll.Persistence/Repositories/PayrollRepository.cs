using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Payrolls;
using QubeFin.Persistence.Models.Hrms;
using QubeFin.Persistence.Models.Payroll;

namespace QubeFin.Payroll.Persistence.Repositories
{
    public interface IPayrollRepository
    {
        Task<PayrollModel?> GetPayrollById(Guid payrollId);
        Task<List<PayrollDetailRow>> GetPayrollDetailByIdAsync(Guid payrollId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PayrollModel>> GetAllPayrolls();
        Task<MonthlyPayroll> GetMonthlyPayrollAsync(int payrollMonth, int payrollYear);
        Task<List<TblPayRoll>> GetPayrollsForUpdateAsync(int month, int year, CancellationToken cancellationToken = default);
        Task<IEnumerable<MonthwisePayrollData>> GetMonthwisePayrollSummaryAsync(Guid? companyId, int? payrollMonth, int payrollYear);
        Task<bool> HasOpenPayrollAsync(Guid companyId, CancellationToken cancellationToken);
        Task CreatePayrollAsync(Guid companyId, Guid? userId, CancellationToken cancellationToken);
        Task<bool> IsPayrollAvailableForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UpdatePayrollComponentsAsync(Guid payrollId, List<PayrollComponentModel> updatedComponents, CancellationToken cancellationToken = default);
        Task<List<Payslip>> GetEmployeePayslipsAsync(Guid employeeId);
        Task<List<TblSalaryGrade>> GetAllSalaryGrade();
    }
    public class PayrollRepository(QubeFinDataContext context) : IPayrollRepository
    {
        public async Task<PayrollModel?> GetPayrollById(Guid payrollId)
        {
            var entity = await context.TblPayRolls.Include(m => m.OrganizationUnit).Include(m => m.Employee).Include(m => m.Designation).Include(m => m.Company)
                .Include(m => m.FinYear)
                .Include(m => m.TblPayRollComponents)
                    .ThenInclude(c => c.SalaryComponent)
                        .ThenInclude(sc => sc.Category)
                        .Include(m => m.SalaryGrade).AsNoTracking().FirstOrDefaultAsync(x => x.Id == payrollId);
            return entity?.ToDomain();
        }
        public async Task<List<PayrollDetailRow>> GetPayrollDetailByIdAsync(Guid payrollId, CancellationToken cancellationToken = default)
        {
            var payrollIdParameter = new SqlParameter("@PayrollId", payrollId);

            // IsEditable is sourced from Tbl_SalaryStructureComponent (matched on the payroll's SalaryStructureId
            // and the component's SalaryComponentId), falling back to Tbl_SalaryComponent when no structure row exists.
            var sql = """
                SELECT
                    p.Id AS Id,
                    p.EmployeeId AS EmployeeId,
                    e.FullName AS EmployeeName,
                    e.Code AS EmployeeCode,
                    p.OrganizationUnitId AS OrganizationUnitId,
                    ou.Name AS OrganizationUnitName,
                    ou.CodeVal AS OrganizationCode,
                    p.DesignationId AS DesignationId,
                    d.Name AS DesignationTitle,
                    p.CompanyId AS CompanyId,
                    c.Name AS CompanyName,
                    fy.Caption AS FinYear,
                    p.PayrollMonth AS PayrollMonth,
                    p.PayrollYear AS PayrollYear,
                    p.IsLocked AS IsLocked,
                    p.DayCount AS DayCount,
                    p.SalaryGradeId AS SalaryGradeId,
                    sg.Name AS SalaryGradeName,
                    p.CreatedOn AS CreatedOn,
                    p.CreatedBy AS CreatedBy,
                    p.SalaryStructureId AS SalaryStructureId,
                    pc.Id AS ComponentId,
                    pc.SalaryComponentId AS SalaryComponentId,
                    sc.Name AS SalaryComponentName,
                    cat.Name AS CategoryName,
                    sc.DisplayOrder AS DisplayOrder,
                    pc.Percentage AS Percentage,
                    pc.Amount AS Amount,
                    COALESCE(ssc.IsEditable, sc.IsEditable) AS IsEditable
                FROM Payroll.Tbl_PayRoll p
                INNER JOIN Hrms.Tbl_Employee e ON e.Id = p.EmployeeId
                INNER JOIN Global.Tbl_OrganizationUnit ou ON ou.Id = p.OrganizationUnitId
                INNER JOIN Hrms.Tbl_Designation d ON d.Id = p.DesignationId
                INNER JOIN Global.Tbl_Company c ON c.Id = p.CompanyId
                INNER JOIN Finance.Tbl_FinancialYear fy ON fy.Id = p.FinYearId
                LEFT JOIN Payroll.Tbl_SalaryGrade sg ON sg.Id = p.SalaryGradeId
                LEFT JOIN Payroll.Tbl_PayRollComponent pc ON pc.PayRollId = p.Id
                LEFT JOIN Payroll.Tbl_SalaryComponent sc ON sc.Id = pc.SalaryComponentId
                LEFT JOIN Payroll.Tbl_SalaryComponentCategory cat ON cat.Id = sc.CategoryId
                LEFT JOIN Payroll.Tbl_SalaryStructureComponent ssc
                    ON ssc.SalaryStructureId = p.SalaryStructureId
                    AND ssc.SalaryComponentId = pc.SalaryComponentId
                WHERE p.Id = @PayrollId
                ORDER BY sc.DisplayOrder
                """;

            return await context.Database
                .SqlQueryRaw<PayrollDetailRow>(sql, payrollIdParameter)
                .ToListAsync(cancellationToken);
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
            return await context.TblPayRolls.AnyAsync(p => p.CompanyId == companyId && p.IsLocked == false, cancellationToken);
        }
        public async Task<IEnumerable<MonthwisePayrollData>> GetMonthwisePayrollSummaryAsync(Guid? companyId, int? payrollMonth, int payrollYear)
        {
            var companyParameter = new SqlParameter("@p_CompanyId", companyId ?? (object?)DBNull.Value);
            var yearParameter = new SqlParameter("@p_Year", payrollYear);
            var monthParameter = new SqlParameter("@p_Month", payrollMonth ?? (object?)DBNull.Value);

            var sql = """
                EXEC Payroll.USP_GetMonthlyPayroll
                    @p_CompanyId = @p_CompanyId,
                    @p_Year = @p_Year,
                    @p_Month = @p_Month
                """;

            var result = await context.Database
                .SqlQueryRaw<MonthwisePayrollData>(
                    sql,
                    companyParameter,
                    yearParameter,
                    monthParameter)
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
        public async Task<List<Payslip>> GetEmployeePayslipsAsync(Guid employeeId)
        {
            return await context.SP_GetEmployeePayslip(employeeId);
        }

        public async Task<List<TblSalaryGrade>> GetAllSalaryGrade()
        {
            return await context.TblSalaryGrades.Include(m => m.TblSalaryStructures).Where(m => m.IsActive).AsNoTracking().ToListAsync();
        }
    }
}
