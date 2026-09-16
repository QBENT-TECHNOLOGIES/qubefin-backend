using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System.Globalization;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetPayrollDetailByIdQuery(Guid PayrollId) : IRequest<Result<GetPayrollByIdResponse>>;
    internal sealed class GetPayrollDetailByIdQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetPayrollDetailByIdQuery, Result<GetPayrollByIdResponse>>
    {
        private const string EarningCategory = "Earning";
        private const string DeductionCategory = "Deduction";

        public async Task<Result<GetPayrollByIdResponse>> Handle(GetPayrollDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var rows = await payrollRepository.GetPayrollDetailByIdAsync(request.PayrollId, cancellationToken);
            if (rows is null || rows.Count == 0) return new RecordNotFoundError("Payroll not found");

            var header = rows[0];

            var components = rows
                .Where(r => r.SalaryComponentId.HasValue)
                .Select(r =>
                {
                    var component = new PayrollComponentModel(
                        r.ComponentId!.Value,
                        header.Id,
                        r.SalaryComponentId!.Value,
                        r.Percentage ?? 0,
                        r.Amount ?? 0,
                        r.IsEditable ?? true);
                    component.SetNames(r.SalaryComponentName ?? string.Empty, r.CategoryName ?? string.Empty, r.DisplayOrder ?? 0);
                    return component;
                })
                .ToList();

            var earnings = components
                .Where(c => string.Equals(c.CategoryName, EarningCategory, StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.DisplayOrder)
                .ToList();

            var deductions = components
                .Where(c => string.Equals(c.CategoryName, DeductionCategory, StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.DisplayOrder)
                .ToList();

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(header.PayrollMonth);
            string monthYear = $"{monthName}, {header.PayrollYear}";

            var response = new GetPayrollByIdResponse(
                Id: header.Id,
                EmployeeId: header.EmployeeId,
                EmployeeName: header.EmployeeName,
                EmployeeCode: header.EmployeeCode,
                OrganizationUnitId: header.OrganizationUnitId,
                OrganizationUnitName: header.OrganizationUnitName,
                OrganizationCode: header.OrganizationCode,
                DesignationId: header.DesignationId,
                DesignationTitle: header.DesignationTitle,
                CompanyId: header.CompanyId,
                CompanyName: header.CompanyName,
                FinYear: header.FinYear,
                PayrollMonth: header.PayrollMonth,
                PayrollYear: header.PayrollYear,
                PayrollMonthYear: monthYear,
                IsLocked: header.IsLocked,
                DayCount: header.DayCount,
                SalaryGradeId: header.SalaryGradeId,
                SalaryGradeName: header.SalaryGradeName,
                CreatedOn: header.CreatedOn,
                CreatedBy: header.CreatedBy,
                SalaryStructureId: header.SalaryStructureId,
                EarningHeads: earnings,
                DeductionHeads: deductions
            );

            return Result.Ok(response);
        }
    }
}
