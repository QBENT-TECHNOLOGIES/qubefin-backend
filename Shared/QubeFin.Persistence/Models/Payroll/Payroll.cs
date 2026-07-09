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

        public Guid DesignationId { get; private set; }
        public string? Designation { get; private set; }
        public Guid CompanyId { get; private set; }
        public string? Company { get; private set; }
        public bool IsLocked { get; private set; }

        public Guid FinYearId { get; private set; }
        public string? FinYear { get; private set; }

        public int? DayCount { get; private set; }
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
        public static PayrollModel Create(
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
            var payroll = new PayrollModel
            {
                Id = id,
                PayrollMonth = payrollMonth,
                PayrollYear = payrollYear,
                EmployeeId = employeeId,
                OrganizationUnitId = organizationUnitId,
                DesignationId = designationId,
                CompanyId = companyId,
                IsLocked = isLocked,
                FinYearId = finYearId,
                DayCount = dayCount
            };
            return payroll;
        }
        public void Update(
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
        public void SetNames(string employeeName, string finYear, string designation, string company)
        {
            EmployeeName = employeeName;
            FinYear = finYear;
            Designation = designation;
            Company = company;
        }
    }
}
