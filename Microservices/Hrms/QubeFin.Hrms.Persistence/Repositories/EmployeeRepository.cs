using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using static QubeFin.Persistence.Models.Hrms.Employee;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id);
        Task CreateEmployeeAsync(Employee employee);
        //void UpdateEmployee(Employee employee);
        //Task<Employee> GetById(Guid employeeId);
        Task<Employee?> GetEmloyeeOrganization(Guid id);
    }
    public class EmployeeRepository(QubeFinDataContext context) : IEmployeeRepository
    {
        public async Task CreateEmployeeAsync(Employee employee)
        {
            await context.TblEmployees.AddAsync(employee.ToEntity());
        }

        //public void UpdateEmployee(Employee employee)
        //{
        //    context.TblEmployees.Update(employee.ToEntity());
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
                .Include(x => x.OrganizationUnit)
                .FirstOrDefaultAsync(x => x.Id == id);

            return employeeEntity is null ? null : employeeEntity.ToDomain();
        }
        public async Task<Employee?> GetEmloyeeOrganization(Guid id)
        {
            var employeeEntity = await context.TblEmployees.Include(m => m.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == id);

            return employeeEntity is null ? null : employeeEntity.ToDomain();
        }
    }
}
