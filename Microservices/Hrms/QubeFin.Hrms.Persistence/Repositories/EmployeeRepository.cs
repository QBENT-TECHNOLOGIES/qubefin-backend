using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;
using static QubeFin.Persistence.Models.Hrms.Employee;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IEmployeeRepository
    {
        Task CreateEmployee(Employee employee);
        Task<EmployeeOrganizationTiming?> GetEmployeeOrganization(Guid employeeId);
    }
    public class EmployeeRepository(QubeFinDataContext context) : IEmployeeRepository
    {
        public Task CreateEmployee(Employee employee)
        {
            context.TblEmployees.Add(employee.ToEntity());
            return Task.CompletedTask;
        }

        public async Task<EmployeeOrganizationTiming?> GetEmployeeOrganization(Guid employeeId)
        {
            var employee = await context.TblEmployees.Include(x => x.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == employeeId);

            if (employee == null)
                return null;

            return new EmployeeOrganizationTiming
            {
                AttendanceInTime = employee.OrganizationUnit?.AttendanceInTime,
                AttendanceOutTime = employee.OrganizationUnit?.AttendanceOutTime,
                OrganizationUnitId = employee.OrganizationUnitId
            };
        }
    }
}
