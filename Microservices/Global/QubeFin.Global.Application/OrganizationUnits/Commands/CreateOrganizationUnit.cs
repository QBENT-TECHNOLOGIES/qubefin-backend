using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnis.Commands;

#region --- COMMAND ---
public record CreateOrganizationUnitCommand(Guid OrganizationUnitTypeId, string Name, Guid? ParentId, Guid? CompanyId, decimal? Latitude, decimal? Longitude,
        TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, int? CheckRadiusInMeter, Guid UserId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class CreateOrganizationUnitCommandValidator : AbstractValidator<CreateOrganizationUnitCommand>
{
    public CreateOrganizationUnitCommandValidator()
    {
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
internal sealed class CreateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<CreateOrganizationUnitCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await organizationUnitRepository.ExistsByNameAsync(request.Name, request.ParentId, null, cancellationToken);
        if (nameExists)
        {
            return Result.Fail($"Organization Unit {request.Name} already exists under the selected parent.");
        }

        var codeVal = await organizationUnitRepository.GetNextCodeValAsync(cancellationToken);

        var organizationUnit = OrganizationUnit.Create(Guid.NewGuid(), request.OrganizationUnitTypeId, request.Name.Trim(), codeVal, request.ParentId, request.CompanyId,
            request.Latitude, request.Longitude, request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, request.UserId);
        await organizationUnitRepository.AddAsync(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok($"Organization Unit {request.Name} Created Successfully");
    }
}
#endregion
