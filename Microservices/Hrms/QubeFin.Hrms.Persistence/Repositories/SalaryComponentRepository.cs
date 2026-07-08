using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Persistence.Repositories
{   
    public interface ISalaryComponentRepository
    {
        Task CreateSalaryComponent(SalaryComponent salaryComponent);
    }
    public class SalaryComponentRepository(QubeFinDataContext context) : ISalaryComponentRepository
    {
        public Task CreateSalaryComponent(SalaryComponent salaryComponent)
        {
            context.TblSalaryComponents.Add(salaryComponent.ToEntity());
            return Task.CompletedTask;
        }
    }
}
