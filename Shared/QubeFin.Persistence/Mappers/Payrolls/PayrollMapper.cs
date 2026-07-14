using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = QubeFin.Persistence.Entities.TblPayRoll;

namespace QubeFin.Persistence.Mappers.Payrolls
{
    public static class PayrollMapper
    {
        private const string EarningCategory = "Earning";
        private const string DeductionCategory = "Deduction";

        public static PayrollModel ToDomain(this Entity entity)
        {
            var domain = new PayrollModel(
                entity.Id,
                entity.PayrollMonth,
                entity.PayrollYear,
                entity.EmployeeId,
                entity.OrganizationUnitId,
                entity.DesignationId,
                entity.CompanyId,
                entity.IsLocked,
                entity.FinYearId,
                entity.DayCount
            );

            string fullName = string.Join(" ", new[]
            {
                entity.Employee.FirstName,
                entity.Employee.MiddleName,
                entity.Employee.LastName
            }.Where(s => !string.IsNullOrWhiteSpace(s)));

            domain.SetNames(entity.OrganizationUnit.Name, entity.OrganizationUnit.CodeVal, fullName, entity.FinYear.Caption, entity.Designation.Name, entity.Company.Name);

            domain.SetComponents(entity.TblPayRollComponents.Select(c =>
            {
                var componentModel = new PayrollComponentModel(c.Id, c.SalaryComponentId, c.Percentage, c.Amount);
                componentModel.SetNames(c.SalaryComponent.Name, c.SalaryComponent.Category.Name);
                return componentModel;
            }));

            return domain;
        }
        public static PayrollLockingModel ToLockingDomain(this Entity entity)
        {
            return new PayrollLockingModel(
                entity.Id,
                entity.IsLocked
            );
        }
        public static void ApplyToEntity(this PayrollLockingModel domain, Entity trackedEntity)
        {
            trackedEntity.IsLocked = domain.IsLocked;
        }
        public static MonthlyPayroll ToMonthlyPayroll(this IEnumerable<PayrollModel> payrolls, int month, int year)
        {
            if (payrolls == null || !payrolls.Any())
            {
                return new MonthlyPayroll
                {
                    PayrollMonth = month,
                    PayrollYear = year,
                    PayrollMonthYear = $"{month:D2}-{year}",
                    IsLocked = false,
                    Headers = new List<MonthlyPayrollHeader>()
                };
            }

            var result = new MonthlyPayroll
            {
                PayrollMonth = month,
                PayrollYear = year,
                PayrollMonthYear = $"{month:D2}-{year}",
                IsLocked = payrolls.First().IsLocked,
                Headers = payrolls
                    .GroupBy(p => p.OrganizationUnitId)
                    .Select(group =>
                    {
                        var details = group.Select(item => new MonthlyPayrollLineItem
                        {
                            Id = item.Id,
                            EmployeeId = item.EmployeeId,
                            EmployeeName = item.EmployeeName,
                            DesignationId = item.DesignationId,
                            DesignationTitle = item.Designation,
                            CompanyId = item.CompanyId,
                            CompanyName = item.Company,
                            TotalEarnings = item.Components
                                .Where(c => string.Equals(c.CategoryName, EarningCategory, StringComparison.OrdinalIgnoreCase))
                                .Sum(c => c.Amount),
                            TotalDeductions = item.Components
                                .Where(c => string.Equals(c.CategoryName, DeductionCategory, StringComparison.OrdinalIgnoreCase))
                                .Sum(c => c.Amount)
                        }).ToList();

                        return new MonthlyPayrollHeader
                        {
                            OrganizationUnitId = group.Key,
                            OrganizationUnitName = group.First().OrganizationUnitName ?? string.Empty,
                            CodeVal = group.First().OrganizationCode,
                            TotalEarnings = details.Sum(d => d.TotalEarnings),
                            TotalDeductions = details.Sum(d => d.TotalDeductions),
                            Details = details
                        };
                    }).ToList()
            };

            return result;
        }
    }
}