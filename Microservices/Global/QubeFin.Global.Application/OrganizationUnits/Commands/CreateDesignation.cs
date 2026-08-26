using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Application.OrganizationUnits.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnits.Commands;

#region --- COMMAND ---
public record CreateDesignationCommand(DesignationRequest designation, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class CreateDesignationCommandValidator : AbstractValidator<CreateDesignationCommand>
{
    public CreateDesignationCommandValidator()
    {
        RuleFor(v => v.designation).NotNull().WithMessage("Designation request is required.");
        RuleFor(v => v.designation.Name)
            .NotEmpty().WithMessage("Designation name is required.")
            .MaximumLength(50).WithMessage("Designation name must not exceed 50 characters.");
        RuleFor(v => v.designation.OrganizationUnitId).NotEmpty().WithMessage("Organization unit ID is required.");
        RuleFor(v => v.designation.PostId).NotEmpty().WithMessage("Post ID is required.");
        RuleFor(v => v.designation.RoleId).NotEmpty().WithMessage("Role ID is required.");
        RuleFor(v => v.designation.SalaryGradeId).NotEmpty().WithMessage("Salary grade ID is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class CreateDesignationCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<CreateDesignationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await organizationUnitRepository.AddDesignationAsync(
                    request.designation.Name, request.designation.OrganizationUnitId, 
                    request.designation.PostId, request.designation.RoleId, 
                    request.designation.SalaryGradeId, request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Designation {request.designation.Name} created successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"{ex.Message}");
        }
    }
}
#endregion
