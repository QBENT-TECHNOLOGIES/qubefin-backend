using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeeOfficialCommand(Guid Id, OfficialInfoRequest OfficialInfo, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeeOfficialCommandValidator : AbstractValidator<UpdateEmployeeOfficialCommand>
{
    public UpdateEmployeeOfficialCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(x => x.OfficialInfo).NotNull().WithMessage("Official Info is required.");
        //RuleFor(x => x.OfficialInfo.CompanyId).NotEmpty();
        RuleFor(x => x.OfficialInfo.OrganizationUnitId).NotEmpty().WithMessage("Organization Unit Id is required.");
        RuleFor(x => x.OfficialInfo.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
        RuleFor(x => x.OfficialInfo.EmployementType).NotEmpty().WithMessage("Employment Type is required.");
        RuleFor(x => x.OfficialInfo.OfficialEmail)
            .NotEmpty().WithMessage("Official Email is required.")
           .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
           .When(x => !string.IsNullOrWhiteSpace(x.OfficialInfo.OfficialEmail))
           .WithMessage("Enter a valid email address.");
        RuleFor(x => x.OfficialInfo.DateOfJoining).NotEmpty().WithMessage("Date of Joining is required.");
        //RuleFor(x => x.OfficialInfo.DateOfConfirmation).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Login User Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeOfficialCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeOfficialCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeeOfficialCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await employeeRepository.GetByIdAsync(request.Id);
            if (employee == null)
            {
                return new ValidationError("Employee does not exist with the given id.");
            }

            employee.UpdateOfficialInfo(
                new OfficialInfo(
                    request.OfficialInfo.CompanyId, request.OfficialInfo.OrganizationUnitId, request.OfficialInfo.DepartmentId, request.OfficialInfo.EmployementType,
                    request.OfficialInfo.DateOfJoining, request.OfficialInfo.DateOfConfirmation, request.OfficialInfo.SeparationDate, request.OfficialInfo.ReferedBy,
                    request.OfficialInfo.HowYouKnow, request.OfficialInfo.OfficialEmail), request.UserId
                );
            await employeeRepository.UpdateAsync(employee);

            if ((employee.Designations == null || !employee.Designations.Any()) && request.OfficialInfo.DesignationId != null)
            {
                await employeeRepository.AddDesignationAsync(employee.Id, request.OfficialInfo.DesignationId.Value, request.OfficialInfo.DateOfJoining.Value);
            }
            if ((employee.GrossSalaries == null || !employee.GrossSalaries.Any()) && request.OfficialInfo.GrossSalary > 0)
            {
                await employeeRepository.AddGrossSalaryAsync(employee.Id, request.OfficialInfo.GrossSalary.Value, request.OfficialInfo.DateOfJoining.Value);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Employee official information updated successfully for Name : {employee.PersonalInfo.FirstName} {employee.PersonalInfo.LastName}");
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"{ex.Message}"));
        }
    }
}
#endregion

