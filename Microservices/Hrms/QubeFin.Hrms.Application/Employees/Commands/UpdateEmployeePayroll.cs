using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.App;
using QubeFin.Persistence.Models.Hrms;
using System.Text.RegularExpressions;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeePayrollCommand(
        Guid Id, Guid BankId, long BankAccountNo, string BankHolderName, string BankBranch, string BankAccountType,
        bool HasEsiEligible, string? EsiIpNumber, string? UniversalAccountNumber, bool IsPayrollActive,
        Guid UserId
    ) : IRequest<Result<UpdateEmployeePayrollResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeePayrollCommandValidator : AbstractValidator<UpdateEmployeePayrollCommand>
{
    public UpdateEmployeePayrollCommandValidator()
    {
            //RuleFor(x => x.FirstName)
            //    .Must(value => !string.IsNullOrWhiteSpace(value)
            //        && Regex.IsMatch(value, @"^[A-Za-z]+$")
            //        && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            //    .WithMessage("Please enter a valid First Name name.")
            //    .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            //    .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
            //RuleFor(x => x.LastName)
            //    .NotEmpty()
            //    .Matches("^[A-Za-z]{3,30}$")
            //    .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");
            
    }
}
#endregion

#region --- RESPONSE ---
public record UpdateEmployeePayrollResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeePayrollCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeePayrollCommand, Result<UpdateEmployeePayrollResponse>>
{
    public async Task<Result<UpdateEmployeePayrollResponse>> Handle(UpdateEmployeePayrollCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdatePayrollInfo(
            new PayrollInfo(request.BankId, request.BankAccountNo, request.BankHolderName, request.BankBranch, request.BankAccountType, request.HasEsiEligible,
                request.EsiIpNumber, request.UniversalAccountNumber, request.IsPayrollActive),
            request.UserId
            );
            //employeeRepository.UpdateEmployee(existingEmployee);

        await employeeRepository.UpdateAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateEmployeePayrollResponse(true));


    }
}
#endregion

