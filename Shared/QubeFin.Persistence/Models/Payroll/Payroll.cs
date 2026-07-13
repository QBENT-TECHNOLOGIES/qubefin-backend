using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Payroll
{
    public class PayrollModel
    {
        public Guid Id { get; private set; }

        public int PayrollMonth { get; private set; }

        public int PayrollYear { get; private set; }

        public Guid EmployeeId { get; private set; }
        public string? EmployeeName { get; private set; }

        public Guid OrganizationUnitId { get; private set; }
        public string? OrganizationUnitName { get; private set; }
        public Guid DesignationId { get; private set; }
        public string? Designation { get; private set; }
        public Guid CompanyId { get; private set; }
        public string? Company { get; private set; }
        public bool IsLocked { get; private set; }

        public Guid FinYearId { get; private set; }
        public string? FinYear { get; private set; }

        public int? DayCount { get; private set; }

        private readonly List<PayrollComponentModel> _components = new();
        public IReadOnlyCollection<PayrollComponentModel> Components => _components.AsReadOnly();

        private PayrollModel() { }
        public PayrollModel(
            Guid id,
            int payrollMonth,
            int payrollYear,
            Guid employeeId,
            Guid organizationUnitId,
            Guid designationId,
            Guid companyId,
            bool isLocked,
            Guid finYearId,
            int? dayCount)
        {
            Id = id;
            PayrollMonth = payrollMonth;
            PayrollYear = payrollYear;
            EmployeeId = employeeId;
            OrganizationUnitId = organizationUnitId;
            DesignationId = designationId;
            CompanyId = companyId;
            IsLocked = isLocked;
            FinYearId = finYearId;
            DayCount = dayCount;
        }
        public void SetNames(string? organizationUnitName, string employeeName, string finYear, string designation, string company)
        {
            OrganizationUnitName = organizationUnitName;
            EmployeeName = employeeName;
            FinYear = finYear;
            Designation = designation;
            Company = company;
        }

        public void SetComponents(IEnumerable<PayrollComponentModel> components)
        {
            _components.Clear();
            _components.AddRange(components);
        }
    }

    public class PayrollComponentModel
    {
        public Guid Id { get; private set; }
        public Guid SalaryComponentId { get; private set; }
        public string? SalaryComponentName { get; private set; }
        public string? CategoryName { get; private set; }
        public decimal Percentage { get; private set; }
        public decimal Amount { get; private set; }

        private PayrollComponentModel() { }

        public PayrollComponentModel(Guid id, Guid salaryComponentId, decimal percentage, decimal amount)
        {
            Id = id;
            SalaryComponentId = salaryComponentId;
            Percentage = percentage;
            Amount = amount;
        }

        public void SetNames(string salaryComponentName, string categoryName)
        {
            SalaryComponentName = salaryComponentName;
            CategoryName = categoryName;
        }
    }

    public class MonthlyPayroll
    {
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public string? PayrollMonthYear { get; set; }
        public bool IsLocked { get; set; }
        public List<MonthlyPayrollHeader>? Headers { get; set; } = new List<MonthlyPayrollHeader>();
    }
    public class MonthlyPayrollHeader
    {
        public Guid OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; } = string.Empty;
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public List<MonthlyPayrollLineItem> Details { get; set; } = new List<MonthlyPayrollLineItem>();
    }
    public class MonthlyPayrollLineItem
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public Guid DesignationId { get; set; }
        public string? DesignationTitle { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
    }
    public class MonthwisePayrollData
    {
        public int PayrollYear { get; set; }
        public int PayrollMonth { get; set; }
        public string? PayrollMonthYear { get; set; }
        public int EmployeeCount { get; set; }
        public decimal Earnings { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }
        public int Locked { get; set; }
    }
}