using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnis.Commands;

#region --- COMMAND ---
public record CreateOrganizationUnitCommand(Guid OrganizationUnitTypeId, string Name, int CodeVal, Guid? ParentId, decimal? Latitude, decimal? Longitude,
        TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, int? CheckRadiusInMeter, Guid UserId) : IRequest<Result<bool>>;
#endregion

#region --- HANDLER ---
internal sealed class CreateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork) : 
    IRequestHandler<CreateOrganizationUnitCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = OrganizationUnit.Create(Guid.NewGuid(), request.OrganizationUnitTypeId, request.Name, request.CodeVal, request.ParentId,
            request.Latitude, request.Longitude, request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, request.UserId);
        await organizationUnitRepository.AddAsync(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(true);
    }
}
#endregion