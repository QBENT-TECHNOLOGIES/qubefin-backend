using QubeFin.Persistence.Models.Payroll;
using Entity = QubeFin.Persistence.Entities.TblPayRoll;

namespace QubeFin.Persistence.Mappers.Payrolls
{
    public static class PayrollMapper
    {
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
            domain.SetNames(fullName, entity.FinYear.Caption, entity.Designation.Name, entity.Company.Name);
            return domain;
        }
        public static Entity ToEntity(this PayrollModel domain)
        {
            return new Entity
            {
                Id = domain.Id,
                PayrollMonth = domain.PayrollMonth,
                PayrollYear = domain.PayrollYear,
                EmployeeId = domain.EmployeeId,
                OrganizationUnitId = domain.OrganizationUnitId,
                DesignationId = domain.DesignationId,
                CompanyId = domain.CompanyId,
                IsLocked = domain.IsLocked,
                FinYearId = domain.FinYearId,
                DayCount = domain.DayCount
            };
        }
    }
}
