using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Commands;

#region --- COMMAND ---
public record CreateAdministrativeUnitCommand(Guid AdministrativeUnitTypeId, string Name, Guid? ParentId) : IRequest<Result<CreateAdministrativeUnitResponse>>;
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
        var administrativeUnit = AdministrativeUnit.Create(Guid.NewGuid(), request.AdministrativeUnitTypeId, request.Name, request.ParentId, Guid.Parse("04360E23-A310-46BA-87D8-89C0EF355E45"));
        administrativeUnitRepository.Add(administrativeUnit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(new CreateAdministrativeUnitResponse(true));
    }
}
#endregion