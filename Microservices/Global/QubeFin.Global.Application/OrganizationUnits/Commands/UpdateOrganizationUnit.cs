using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Commands;

#region --- COMMAND ---
public record UpdateOrganizationUnitCommand(Guid Id, Guid OrganizationUnitTypeId, string Name, decimal? Latitude, decimal? Longitude,
    TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, int? CheckRadiusInMeter, Guid? ParentId, Guid? CompanyId, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateOrganizationUnitCommandValidator : AbstractValidator<UpdateOrganizationUnitCommand>
{
    public UpdateOrganizationUnitCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Organization unit is required.");
        RuleFor(v => v.OrganizationUnitTypeId).NotEmpty().WithMessage("Organization unit type is required.");
        RuleFor(v => v.Name).NotEmpty().WithMessage("Organization unit name is required.");
        RuleFor(v => v.Name).MaximumLength(50).WithMessage("Organization unit name cannot exceed 50 characters.");
        RuleFor(v => v.CheckRadiusInMeter).GreaterThan(0).When(v => v.CheckRadiusInMeter.HasValue).WithMessage("Check radius must be greater than 0.");
        RuleFor(v => v.Latitude).InclusiveBetween(-90, 90).When(v => v.Latitude.HasValue).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(v => v.Longitude).InclusiveBetween(-180, 180).When(v => v.Longitude.HasValue).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrganizationUnitCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = await organizationUnitRepository.GetByIdAsync(request.Id);
        if (organizationUnit is null)
        {
            return new RecordNotFoundError($"Organization Unit not found for the given Id");
        }

        if (request.ParentId.HasValue)
        {
            var createsCycle = await organizationUnitRepository.IsSelfOrDescendantAsync(request.ParentId.Value, request.Id, cancellationToken);
            if (createsCycle)
            {
                return Result.Fail("The selected parent belongs to this organization unit's own hierarchy.");
            }
        }

        var nameExists = await organizationUnitRepository.ExistsByNameAsync(request.Name, request.ParentId, request.Id, cancellationToken);
        if (nameExists)
        {
            return Result.Fail($"Organization Unit {request.Name} already exists under the selected parent.");
        }

        organizationUnit.Update(request.OrganizationUnitTypeId, request.Name.Trim(), request.Latitude, request.Longitude, request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, request.ParentId, request.CompanyId, request.UserId);
        organizationUnitRepository.Update(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Organization Unit Updated Successfully");
    }
}
#endregion
