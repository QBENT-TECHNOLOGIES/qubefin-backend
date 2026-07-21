using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetPayrollByIdQuery(Guid PayrollId) : IRequest<Result<GetPayrollByIdResponse>>;
    public record GetPayrollByIdResponse(Guid Id,
        Guid EmployeeId,
        string? EmployeeName,
        string? EmployeeCode,
        Guid OrganizationUnitId,
        string? OrganizationUnitName,
        int? OrganizationCode,
        Guid DesignationId,
        string? DesignationTitle,
        Guid CompanyId,
        string? CompanyName,
        string? FinYear,
        int PayrollMonth,
        int PayrollYear,
        string PayrollMonthYear,
        bool IsLocked,
        int? DayCount,
        Guid? SalaryGradeId,
        string? SalaryGradeName,
        DateTime? CreatedOn,
        Guid? CreatedBy,
        Guid? SalaryStructureId,
        IEnumerable<PayrollComponentModel> EarningHeads,
        IEnumerable<PayrollComponentModel> DeductionHeads);
    internal sealed class GetPayrollByIdQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetPayrollByIdQuery, Result<GetPayrollByIdResponse>>
    {
        public async Task<Result<GetPayrollByIdResponse>> Handle(GetPayrollByIdQuery request, CancellationToken cancellationToken)
        {
            var payroll = await payrollRepository.GetPayrollById(request.PayrollId);
            if (payroll is null) return new RecordNotFoundError("Payroll not found");
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(payroll.PayrollMonth);
            string monthYear = $"{monthName}, {payroll.PayrollYear}";
            var earnings = payroll.Components
                .Where(c => string.Equals(c.CategoryName, "Earning", StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.DisplayOrder)
                .ToList();

            var deductions = payroll.Components
                .Where(c => string.Equals(c.CategoryName, "Deduction", StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.DisplayOrder)
                .ToList();
            var response = new GetPayrollByIdResponse(
                Id: payroll.Id,
                EmployeeId: payroll.EmployeeId,
                EmployeeName: payroll.EmployeeName,
                EmployeeCode: payroll.EmployeeCode,
                OrganizationUnitId: payroll.OrganizationUnitId,
                OrganizationUnitName: payroll.OrganizationUnitName,
                OrganizationCode: payroll.OrganizationCode,
                DesignationId: payroll.DesignationId,
                DesignationTitle: payroll.Designation,
                CompanyId: payroll.CompanyId,
                CompanyName: payroll.Company,
                FinYear: payroll.FinYear,
                PayrollMonth: payroll.PayrollMonth,
                PayrollYear: payroll.PayrollYear,
                PayrollMonthYear: monthYear,
                IsLocked: payroll.IsLocked,
                DayCount: payroll.DayCount,
                SalaryGradeId: payroll.SalaryGradeId,
                SalaryGradeName: payroll.SalaryGradeName,
                CreatedOn: payroll.CreatedOn,
                CreatedBy: payroll.CreatedBy,
                SalaryStructureId: payroll.SalaryStructureId,
                EarningHeads: earnings,
                DeductionHeads: deductions
            );
            return Result.Ok(response);
        }
    }
}
