using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetSalaryStructureByGrossQuery(Guid EmployeeId, Guid SalaryGradeId, decimal GrossSalary, decimal? FixedPFamount)
    : IRequest<Result<EmployeeGrossSalaryStructureResponse>>;
#endregion

#region --- VALIDATION ---
public class GetSalaryStructureByGrossValidator : AbstractValidator<GetSalaryStructureByGrossQuery>
{
    public GetSalaryStructureByGrossValidator()
    {
        RuleFor(r => r.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(r => r.SalaryGradeId).NotEmpty().WithMessage("Salary grade Id is required.");
        RuleFor(r => r.GrossSalary).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Gross salary should be greater than 0");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSalaryStructureByGrossQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetSalaryStructureByGrossQuery, Result<EmployeeGrossSalaryStructureResponse>>
{
    private const string EarningCategory = "Earning";
    private const string DeductionCategory = "Deduction";
    public async Task<Result<EmployeeGrossSalaryStructureResponse>> Handle(GetSalaryStructureByGrossQuery request, CancellationToken cancellationToken)
    {
        var salaryStructure = await employeeRepository.GetSalaryStructureByGrossAsync(request.EmployeeId, request.SalaryGradeId, request.GrossSalary, request.FixedPFamount, cancellationToken);
        var earnings = salaryStructure
                .Where(c => string.Equals(c.Category, EarningCategory, StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.DisplayOrder)
                .ToList();

        var deductions = salaryStructure
            .Where(c => string.Equals(c.Category, DeductionCategory, StringComparison.OrdinalIgnoreCase))
            .OrderBy(o => o.DisplayOrder)
            .ToList();
        var employeeDetails = salaryStructure != null && salaryStructure.Any() ? salaryStructure.First() : null;
        var response = new EmployeeGrossSalaryStructureResponse
        {
            EmployeeName = employeeDetails?.EmployeeName,
            EmployeeCode = employeeDetails?.EmployeeCode,
            OrganizationUnitName = employeeDetails?.OrganizationUnitName,
            DesignationTitle = employeeDetails?.DesignationTitle,
            SalaryGradeName = employeeDetails?.SalaryGradeName,
            EarningHeads = earnings.Select(c => new EmployeeGrossSalaryComponent
            {
                CategoryName = c.Category,
                SalaryComponentName = c.SalaryComponentName,
                Percentage = c.Percentage,
                Amount = c.MonthlyAmount
            }).ToList(),
            DeductionHeads = deductions.Select(c => new EmployeeGrossSalaryComponent
            {
                CategoryName = c.Category,
                SalaryComponentName = c.SalaryComponentName,
                Percentage = c.Percentage,
                Amount = c.MonthlyAmount
            }).ToList()
        };
        return Result.Ok(response);
    }
}
#endregion
