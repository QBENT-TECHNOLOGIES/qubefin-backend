using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---
public record UpdateEmployeeOfficialCommand(
        Guid Id, Guid? CompanyId, Guid? OrganizationUnitId, Guid? DepartmentId, string? EmployementType, DateOnly? DateOfJoining, DateOnly? DateOfConfirmation,
        DateOnly? SeparationDate, Guid? ReferedBy, string? HowYouKnow, string? OfficialEmail, bool IsActive,
        Guid UserId
    ) : IRequest<Result<UpdateEmployeeOfficialResponse>>;
#endregion

#region --- VALIDATION ---
public class UpdateEmployeeOfficialCommandValidator : AbstractValidator<UpdateEmployeeOfficialCommand>
{
    public UpdateEmployeeOfficialCommandValidator()
    {
    }
}
#endregion

#region --- RESPONSE ---
public record UpdateEmployeeOfficialResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class UpdateEmployeeOfficialCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeOfficialCommand, Result<UpdateEmployeeOfficialResponse>>
{
    public async Task<Result<UpdateEmployeeOfficialResponse>> Handle(UpdateEmployeeOfficialCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);
        if (employee == null)
        {
            return new ValidationError("Employee does not exist with the given id.");
        }

        employee.UpdateOfficialInfo(
            new OfficialInfo(request.CompanyId, request.OrganizationUnitId, request.DepartmentId, request.EmployementType, request.DateOfJoining, request.DateOfConfirmation,
                request.SeparationDate, request.ReferedBy, request.HowYouKnow, request.OfficialEmail, request.IsActive),
            request.UserId
            );

        await employeeRepository.UpdateAsync(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateEmployeeOfficialResponse(true));
    }
}
#endregion

