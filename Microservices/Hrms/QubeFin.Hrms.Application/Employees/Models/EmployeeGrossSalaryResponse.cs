using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Employees.Models
{
    public class EmployeeGrossSalaryHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid? SalaryGradeId { get; set; }
        public decimal GrossSalary { get; set; } = 0;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTill { get; set; }
    }

    public class EmployeeCurrentGrossSalaryResponse
    {
        public Guid? Id { get; set; }
        public Guid? SalaryGradeId { get; set; }
        public decimal? GrossSalary { get; set; } = 0;
        public DateOnly? EffectiveFrom { get; set; }
        public DateOnly? EffectiveTill { get; set; }
    }

    public class EmployeeGrossSalaryRequest
    {
        public Guid EmployeeId { get; set; }
        public Guid SalaryGradeId { get; set; }
        public decimal GrossSalary { get; set; } = 0;
        public decimal? PfAmount { get; set; }
        public DateOnly EffectiveFrom { get; set; }
    }
    public class EmployeeGrossSalaryStructureResponse
    {
        public List<EmployeeGrossSalaryComponent> EarningHeads { get; set; } = new List<EmployeeGrossSalaryComponent>();
        public List<EmployeeGrossSalaryComponent> DeductionHeads { get; set; } = new List<EmployeeGrossSalaryComponent>();
    }
    public class EmployeeGrossSalaryComponent
    {
        public string CategoryName { get; set; } = string.Empty;
        public string SalaryComponentName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public decimal Amount { get; set; }
    }
}
