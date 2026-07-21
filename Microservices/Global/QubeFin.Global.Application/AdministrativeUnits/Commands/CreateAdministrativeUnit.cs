using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Commands;

#region --- COMMAND ---
public record CreateAdministrativeUnitCommand(Guid AdministrativeUnitTypeId, string Name, Guid? ParentId, Guid UserId) : IRequest<Result<CreateAdministrativeUnitResponse>>;
#endregion

#region --- RESPONSE ---
public record CreateAdministrativeUnitResponse(bool Created);
#endregion

#region --- HANDLER ---
internal sealed class CreateAdministrativeUnitCommandHandler(IAdministrativeUnitRepository administrativeUnitRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAdministrativeUnitCommand, Result<CreateAdministrativeUnitResponse>>
{
    public async Task<Result<CreateAdministrativeUnitResponse>> Handle(CreateAdministrativeUnitCommand request, CancellationToken cancellationToken)
    {
        var administrativeUnit = AdministrativeUnit.Create(Guid.NewGuid(), request.AdministrativeUnitTypeId, request.Name, request.ParentId, request.UserId);
        await administrativeUnitRepository.AddAsync(administrativeUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateAdministrativeUnitResponse(true));
    }
}
#endregion