using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories
{
    public interface IEmployeeRepository
    {
        Task CreateEmployee(Employee employee);
    }
    public class EmployeeRepository(QubeFinDataContext context) : IEmployeeRepository
    {
        public Task CreateEmployee(Employee employee)
        {
            context.TblEmployees.Add(employee.ToEntity());
            return Task.CompletedTask;
        }
    }
}
