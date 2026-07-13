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
public record UpdateEmployeePersonalCommand(
    Guid Id, string? Salutation, string FirstName, string? MiddleName, string LastName, string? FatherName, string? MotherName,
    DateOnly DateOfBirth, string Gender, string Religion, string? Caste, string Nationality, string BloodGroup, string? DisabilityType, string? MaritalStatus,
    Guid UserId
    ) : IRequest<Result<UpdateEmployeePersonalResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeePersonalCommandValidator : AbstractValidator<UpdateEmployeePersonalCommand>
{
    public UpdateEmployeePersonalCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .Must(value => !string.IsNullOrWhiteSpace(value)
                && Regex.IsMatch(value, @"^[A-Za-z]+$")
                && !value.Equals("Select", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Please enter a valid First Name name.")
            .MinimumLength(3).WithMessage("First Name must be more than 2 characters.")
            .MaximumLength(30).WithMessage("First Name cannot exceed 30 characters.");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .Matches("^[A-Za-z]{3,30}$")
            .WithMessage("Last name must contain only letters and be between 3 and 30 characters long.");

    }
}
#endregion

#region --- RESPONSE ---
public record UpdateEmployeePersonalResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeePersonalCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeePersonalCommand, Result<UpdateEmployeePersonalResponse>>
{
    public async Task<Result<UpdateEmployeePersonalResponse>> Handle(UpdateEmployeePersonalCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdatePersonalInfo(
            new PersonalInfo(request.Salutation, request.FirstName, request.MiddleName, request.LastName, request.FatherName, request.MotherName,
                request.DateOfBirth, request.Gender, request.Religion, request.Caste, request.Nationality, request.BloodGroup, request.DisabilityType, request.MaritalStatus),
            request.UserId
            );

        await employeeRepository.UpdateAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateEmployeePersonalResponse(true));
    }
}
#endregion
