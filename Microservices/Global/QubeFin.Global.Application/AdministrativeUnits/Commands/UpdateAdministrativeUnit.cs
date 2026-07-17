using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.AdministrativeUnits.Commands;

#region --- COMMAND ---
public record UpdateAdministrativeUnitCommand(Guid Id, Guid AdministrativeUnitTypeId, string Name, Guid? ParentId, Guid UserId) : IRequest<Result<UpdateAdministrativeUnitResponse>>;
#endregion

#region --- RESPONSE ---
public record UpdateAdministrativeUnitResponse(bool Updated);
#endregion

#region --- HANDLER ---
internal sealed class UpdateAdministrativeUnitCommandHandler(IAdministrativeUnitRepository administrativeUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAdministrativeUnitCommand, Result<UpdateAdministrativeUnitResponse>>
{
    public async Task<Result<UpdateAdministrativeUnitResponse>> Handle(UpdateAdministrativeUnitCommand request, CancellationToken cancellationToken)
    {
        var administrativeUnit = await administrativeUnitRepository.GetByIdAsync(request.Id);
        if (administrativeUnit is null)
        {
            return new RecordNotFoundError($"Administrative Unit not found for the given Id");
        }

        administrativeUnit.Update(request.AdministrativeUnitTypeId, request.Name, request.ParentId, request.UserId);
        administrativeUnitRepository.Update(administrativeUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new UpdateAdministrativeUnitResponse(true));
    }
}
#endregion