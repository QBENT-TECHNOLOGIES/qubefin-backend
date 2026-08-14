using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Commands;

#region --- COMMAND ---
public record UpdateOrganizationUnitCommand(Guid Id, Guid OrganizationUnitTypeId, string Name, int CodeVal, decimal? Latitude, decimal? Longitude,
    TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, int? CheckRadiusInMeter, Guid? ParentId, Guid UserId) : IRequest<Result<bool>>;
#endregion

#region --- HANDLER ---
internal sealed class UpdateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrganizationUnitCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = await organizationUnitRepository.GetByIdAsync(request.Id);
        if (organizationUnit is null)
        {
            return new RecordNotFoundError($"Organization Unit not found for the given Id");
        }

        organizationUnit.Update(request.OrganizationUnitTypeId, request.Name, request.CodeVal, request.Latitude, request.Longitude, request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, request.ParentId, request.UserId);
        organizationUnitRepository.Update(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(true);
    }
}
#endregion
