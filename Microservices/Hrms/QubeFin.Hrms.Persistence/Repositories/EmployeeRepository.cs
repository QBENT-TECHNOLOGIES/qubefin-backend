using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task<Employee?> GetEmloyeeOrganization(Guid id);
    Task<bool> GetExsitingEmployeeByCode(Guid? id, string code);
    Task AddDesignationAsync(Guid employeeId, Guid designationId);
    Task AddGrossSalaryAsync(Guid employeeId, decimal grossSalary);
}
public class EmployeeRepository(QubeFinDataContext context) : IEmployeeRepository
{
    public async Task AddAsync(Employee employee)
    {
        await context.TblEmployees.AddAsync(employee.ToEntity());
    }

    public async Task UpdateAsync(Employee employee)
    {
        context.TblEmployees.Update(employee.ToEntity());
    }

    //public async Task<EmployeeOrganizationTiming?> GetEmployeeOrganization(Guid employeeId)
    //{
    //    var employee = await context.TblEmployees.Include(x => x.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == employeeId);

    //    if (employee == null)
    //        return null;

    //    return new EmployeeOrganizationTiming
    //    {
    //        AttendanceInTime = employee.OrganizationUnit?.AttendanceInTime,
    //        AttendanceOutTime = employee.OrganizationUnit?.AttendanceOutTime,
    //        OrganizationUnitId = employee.OrganizationUnitId
    //    };
    //}
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        var employeeEntity = await context.TblEmployees
            .Include(x => x.TblEmployeeDesignations)
            .Include(x => x.TblEmployeeQualifications)
            .Include(x => x.TblEmployeeEmployments)
            .Include(x => x.TblEmployeeDocuments)
            .Include(x => x.TblEmployeeReferences)
            .Include(x => x.TblEmployeeGrossSalaries)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }
        
    public async Task<Employee?> GetEmloyeeOrganization(Guid id)
    {
        var employeeEntity = await context.TblEmployees.Include(m => m.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }

    public async Task<bool> GetExsitingEmployeeByCode(Guid? id, string code)
    {
        return await context.TblEmployees.AsNoTracking().AnyAsync(x => x.Code.ToLower().Trim() == code.ToLower().Trim() && (id == null || id == Guid.Empty || x.Id != id));
    }
    public async Task AddDesignationAsync(Guid employeeId, Guid designationId)
    {
        var employeeDesignation = new TblEmployeeDesignation
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            DesignationId = designationId,
            EffectiveFrom = DateTime.UtcNow,
            EffectiveTo = null
        };
        await context.TblEmployeeDesignations.AddAsync(employeeDesignation);
    }
    public async Task AddGrossSalaryAsync(Guid employeeId, decimal grossSalary)
    {
        var employeeGrossSalary = new TblEmployeeGrossSalary
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            GrossSalary = grossSalary,
            EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            EffectiveTill = null
        };
        await context.TblEmployeeGrossSalaries.AddAsync(employeeGrossSalary);
    }
}

