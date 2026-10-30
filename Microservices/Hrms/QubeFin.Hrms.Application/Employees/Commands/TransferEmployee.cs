using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Commands;

#region --- COMMAND ---

public record TransferEmployeeCommand(EmployeeCurrentOfficialInfoRequest employee, Guid UserId) : IRequest<Result<string>>;

#endregion

#region --- VALIDATION ---
public class TransferEmployeeCommandValidator : AbstractValidator<TransferEmployeeCommand>
{
    public TransferEmployeeCommandValidator()
    {
        RuleFor(x => x.employee.EmployeeId).NotEmpty().WithMessage("Employee is required.");
        RuleFor(x => x.employee.OrganisationUnitId).NotEmpty().WithMessage("Organisation Unit is required.");
        RuleFor(x => x.employee.EffectiveFrom).NotEmpty().WithMessage("Effective From date is required.");
    }
}
#endregion

#region --- HANDLER ---

internal sealed class TransferEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork) : IRequestHandler<TransferEmployeeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(TransferEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await employeeRepository.TransferEmployee(request.employee.EmployeeId, request.employee.OrganisationUnitId, request.employee.EffectiveFrom, request.UserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok($"Employee transferred successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

#endregion
