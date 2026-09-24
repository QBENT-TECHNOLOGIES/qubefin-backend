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
        public DateOnly EffectiveFrom { get; set; }
    }
}
