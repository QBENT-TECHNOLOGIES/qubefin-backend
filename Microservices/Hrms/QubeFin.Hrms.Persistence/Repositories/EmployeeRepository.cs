using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    //Task<Employee> GetById(Guid employeeId);
    Task<Employee?> GetEmloyeeOrganization(Guid id);
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
    //public async Task<Employee> GetById(Guid employeeId)
    //{
    //    var existingEmployee = await context.TblEmployees.FirstOrDefaultAsync(m => m.Id == employeeId);
    //    if (existingEmployee == null)
    //    {
    //        return null;
    //    }

    //    return existingEmployee.ToDomain();

    //}
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        var employeeEntity = await context.TblEmployees
            .Include(x => x.TblEmployeeDesignations)
            .Include(x => x.TblEmployeeQualifications)
            .Include(x => x.TblEmployeeEmployments)
            .Include(x => x.TblEmployeeDocuments)
            .Include(x => x.TblEmployeeReferences)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }
        
    public async Task<Employee?> GetEmloyeeOrganization(Guid id)
    {
        var employeeEntity = await context.TblEmployees.Include(m => m.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }
}

