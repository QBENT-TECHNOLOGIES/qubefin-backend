using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Holidays.Commands;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Departments.Commands;

    public record UpdateDepartmentCommand(
        Guid Id,
        string Name,
        bool IsActive,
        Guid ModifiedBy) : IRequest<Result<string>>;
public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
internal sealed class UpdateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDepartmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(request.Id);
        if (department is null)
        {
            return new RecordNotFoundError("department not found.");
        }

        department.Update(request.Name.Trim(), request.IsActive, request.ModifiedBy);
        await departmentRepository.UpdateAsync(department);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{request.Name} Holiday updated successfully.");
    }
}
