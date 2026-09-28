using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeSalaryStructure
    {
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? OrganizationUnitName { get; set; }
        public string? DesignationTitle { get; set; }
        public string? SalaryGradeName { get; set; }
        public Guid SalaryComponentId { get; set; }
        public string SalaryComponentName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int NoOfDaysInMonth { get; set; }
        public int PayRollDayCount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ComponentCode { get; set; } = string.Empty;
        public decimal FixedAmount { get; set; }
        public int DisplayOrder { get; set; }
    }
}
