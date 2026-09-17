using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---

public record SaveEmployeeGrossSalaryCommand(EmployeeGrossSalaryRequest employeeGrossSalary) : IRequest<Result<string>>;

#endregion

#region --- VALIDATION ---
public class SaveEmployeeGrossSalaryCommandValidator : AbstractValidator<SaveEmployeeGrossSalaryCommand>
{
    public SaveEmployeeGrossSalaryCommandValidator()
    {
        RuleFor(x => x.employeeGrossSalary.EmployeeId).NotEmpty().WithMessage("Employee is required.");
        RuleFor(x => x.employeeGrossSalary.SalaryGradeId).NotEmpty().WithMessage("Salary Grade is required.");
        RuleFor(x => x.employeeGrossSalary.EffectiveFrom).NotEmpty().WithMessage("Effective From is required.");
        RuleFor(x => x.employeeGrossSalary.GrossSalary).GreaterThanOrEqualTo(0).WithMessage("Gross Salary cannot be negative.");
    }
}
#endregion

#region --- HANDLER ---

internal sealed class SaveEmployeeGrossSalaryCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : IRequestHandler<SaveEmployeeGrossSalaryCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SaveEmployeeGrossSalaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await employeeRepository.SaveGrossSalary(request.employeeGrossSalary.EmployeeId, request.employeeGrossSalary.SalaryGradeId, request.employeeGrossSalary.GrossSalary, request.employeeGrossSalary.EffectiveFrom, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok($"Employee gross salary saved successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

#endregion
