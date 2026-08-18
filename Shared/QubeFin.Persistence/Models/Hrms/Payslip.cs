using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class Payslip
    {
        public Guid? PayslipId { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; } = string.Empty;
        public string? Designation { get; set; } = string.Empty;
        public string? OrganizationUnitName { get; set; } = string.Empty;
        public string? SalaryGrade { get; set; } = string.Empty;
        public string? PayrollMonthYear { get; set; }
        public decimal? TotalEarning { get; set; }
        public decimal? TotalDeduction { get; set; }
        public decimal? NetPay { get; set; }
    }
}
