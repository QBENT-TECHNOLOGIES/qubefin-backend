using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeSalaryStructure
    {
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
