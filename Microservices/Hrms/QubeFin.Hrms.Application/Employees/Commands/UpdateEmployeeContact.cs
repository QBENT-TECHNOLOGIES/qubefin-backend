using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeeContactCommand(
    Guid Id, string MobileNo, string? PersonalEmail, string? PrimaryEmergencyRelation, string? PrimaryEmergencyName, string? PrimaryEmergencyMobile,
        string? SecondaryEmergencyRelation, string? SecondaryEmergencyName, string? SecondaryEmergencyMobile,
    Guid UserId
    ) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeeContactCommandValidator : AbstractValidator<UpdateEmployeeContactCommand>
{
    public UpdateEmployeeContactCommandValidator()
    {
        
        RuleFor(x => x.MobileNo)
            .NotEmpty().WithMessage("Mobile No is required.")
            .Matches(@"^[6-9]\d{9}$").WithMessage("Enter a valid 10-digit mobile number.");

        RuleFor(x => x.PersonalEmail)
            .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PersonalEmail))
            .WithMessage("Enter a valid email address.");

        RuleFor(x => x.PrimaryEmergencyMobile)
            .Matches(@"^[6-9]\d{9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PrimaryEmergencyMobile))
            .WithMessage("Enter a valid 10-digit mobile number for Primary Emergency Contact.");

        RuleFor(x => x.SecondaryEmergencyMobile)
            .Matches(@"^[6-9]\d{9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmergencyMobile))
            .WithMessage("Enter a valid 10-digit mobile number for Secondary Emergency Contact.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeContactCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeContactCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeeContactCommand request, CancellationToken cancellationToken)
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
        return Result.Ok($"Employee contact information updated successfully for Name : {employee.PersonalInfo.FirstName} {employee.PersonalInfo.LastName}");
    }
}
#endregion
