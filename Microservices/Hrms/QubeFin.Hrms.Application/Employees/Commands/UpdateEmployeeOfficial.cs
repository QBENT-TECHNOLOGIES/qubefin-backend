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
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OfficialInfo).NotNull();
        //RuleFor(x => x.OfficialInfo.CompanyId).NotEmpty();
        RuleFor(x => x.OfficialInfo.OrganizationUnitId).NotEmpty();
        //RuleFor(x => x.OfficialInfo.DepartmentId).NotEmpty();
        RuleFor(x => x.OfficialInfo.EmployementType).NotEmpty();
        RuleFor(x => x.OfficialInfo.OfficialEmail).NotEmpty();
        RuleFor(x => x.OfficialInfo.DateOfJoining).NotEmpty();
        //RuleFor(x => x.OfficialInfo.DateOfConfirmation).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeOfficialCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeOfficialCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateEmployeeOfficialCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdateOfficialInfo(
            new OfficialInfo(request.OfficialInfo.CompanyId, request.OfficialInfo.OrganizationUnitId, request.OfficialInfo.DepartmentId, request.OfficialInfo.EmployementType, request.OfficialInfo.DateOfJoining, request.OfficialInfo.DateOfConfirmation,
                request.OfficialInfo.SeparationDate, request.OfficialInfo.ReferedBy, request.OfficialInfo.HowYouKnow, request.OfficialInfo.OfficialEmail, request.OfficialInfo.IsActive),
            request.UserId
            );
        await employeeRepository.UpdateAsync(employee);



        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Employee official information updated successfully for Name : {employee.PersonalInfo.FirstName} {employee.PersonalInfo.LastName}");
    }
}
#endregion

