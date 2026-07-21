using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Commands;

#region --- COMMAND ---
public record UpdateOrganizationUnitCommand(Guid Id, Guid OrganizationUnitTypeId, string Name, int CodeVal, TimeOnly? AttendanceInTime,
        TimeOnly? AttendanceOutTime, Guid? ParentId, Guid UserId) : IRequest<Result<UpdateOrganizationUnitResponse>>;
#endregion

#region --- RESPONSE ---
public record UpdateOrganizationUnitResponse(bool Updated);
#endregion

#region --- HANDLER ---
internal sealed class UpdateOrganizationUnitCommandHandler(IOrganizationUnitRepository organizationUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateOrganizationUnitCommand, Result<UpdateOrganizationUnitResponse>>
{
    public async Task<Result<UpdateOrganizationUnitResponse>> Handle(UpdateOrganizationUnitCommand request, CancellationToken cancellationToken)
    {
        var organizationUnit = await organizationUnitRepository.GetByIdAsync(request.Id);
        if (organizationUnit is null)
        {
            return new RecordNotFoundError($"Organization Unit not found for the given Id");
        }

        organizationUnit.Update(request.OrganizationUnitTypeId, request.Name, request.CodeVal, request.AttendanceInTime, request.AttendanceOutTime, request.ParentId, request.UserId);
        organizationUnitRepository.Update(organizationUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateOrganizationUnitResponse(true));
    }
}
#endregion
