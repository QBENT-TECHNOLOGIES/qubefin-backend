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
public record UpdateEmployeeContactCommand(
    Guid Id, string MobileNo, string? PersonalEmail, string? PrimaryEmergencyRelation, string? PrimaryEmergencyName, string? PrimaryEmergencyMobile,
        string? SecondaryEmergencyRelation, string? SecondaryEmergencyName, string? SecondaryEmergencyMobile,
    Guid UserId
    ) : IRequest<Result<UpdateEmployeeContactResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeeContactCommandValidator : AbstractValidator<UpdateEmployeeContactCommand>
{
    public UpdateEmployeeContactCommandValidator()
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
public record UpdateEmployeeContactResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeContactCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeContactCommand, Result<UpdateEmployeeContactResponse>>
{
    public async Task<Result<UpdateEmployeeContactResponse>> Handle(UpdateEmployeeContactCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdateContactInfo(
            new ContactInfo(request.MobileNo, request.PersonalEmail, request.PrimaryEmergencyRelation, request.PrimaryEmergencyName, request.PrimaryEmergencyMobile,
            request.SecondaryEmergencyRelation, request.SecondaryEmergencyName, request.SecondaryEmergencyMobile),
            request.UserId
            );

        await employeeRepository.UpdateAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateEmployeeContactResponse(true));
    }
}
#endregion
