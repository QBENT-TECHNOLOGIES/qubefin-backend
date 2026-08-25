using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeePayrollCommand(
        Guid Id, Guid BankId, string BankHolderName, long BankAccountNo, string IfscCode, string BankBranch, string? BankAccountType,
        bool HasEsiEligible, string? EsiIpNumber, string? UniversalAccountNumber, string? PFAccountNo, bool IsPayrollActive,
        Guid UserId
    ) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeePayrollCommandValidator : AbstractValidator<UpdateEmployeePayrollCommand>
{
    public UpdateEmployeePayrollCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(x => x.BankId).NotEmpty().WithMessage("Bank Id is required.");
        RuleFor(x => x.BankAccountNo)
            .NotEmpty().WithMessage("Bank account number is required.")
            .Must(accountNo => Regex.IsMatch(accountNo.ToString(), @"^\d{9,15}$"))
            .WithMessage("Account number must be between 9 and 15 digits.");
        RuleFor(x => x.IfscCode)
            .NotEmpty().WithMessage("IFSC Code is required.")
            .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$")
            .WithMessage("IFSC Code must be in the format: 4 uppercase letters, followed by '0', followed by 6 alphanumeric characters.");

        RuleFor(x => x.UniversalAccountNumber)
         .Matches(@"^\d{12}$")
         .When(x => !string.IsNullOrWhiteSpace(x.UniversalAccountNumber))
         .WithMessage("Universal Account Number must contain exactly 12 digits.");
        RuleFor(x => x.PFAccountNo)
         .Matches(@"^\d{7,15}$")
         .When(x => !string.IsNullOrWhiteSpace(x.PFAccountNo))
         .WithMessage("PF Account Number must contain between 7 and 15 digits.");
        RuleFor(x => x.EsiIpNumber).NotEmpty().When(x => x.HasEsiEligible).WithMessage("ESI IP Number is required when ESI Eligible is true.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeePayrollCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeePayrollCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeePayrollCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdatePayrollInfo(
            new PayrollInfo(request.BankId, request.BankHolderName, request.BankAccountNo, request.IfscCode, request.BankBranch, request.BankAccountType,
            request.UniversalAccountNumber, request.PFAccountNo, request.HasEsiEligible, request.EsiIpNumber, request.IsPayrollActive), request.UserId);
        //employeeRepository.UpdateEmployee(existingEmployee);

        await employeeRepository.UpdateAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Employee payroll information updated successfully for Name : {employee.PersonalInfo.FirstName} {employee.PersonalInfo.LastName}");
    }
}
#endregion

