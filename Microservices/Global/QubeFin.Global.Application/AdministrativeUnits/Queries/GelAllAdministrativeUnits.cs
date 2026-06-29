using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

#region --- QUERY ---
public record GetAdministrativeUnitsQuery : IRequest<Result<List<AdministrativeUnitTree>>>;
#endregion

#region --- HANDLER ---
internal sealed class GetAdministrativeUnitsQueryHandler(IAdministrativeUnitRepository administrativeUnitRepository)
    : IRequestHandler<GetAdministrativeUnitsQuery, Result<List<AdministrativeUnitTree>>>
{
    public async Task<Result<List<AdministrativeUnitTree>>> Handle(GetAdministrativeUnitsQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnits = await administrativeUnitRepository.GetAllAsync(cancellationToken);
        return Result.Ok(AdministrativeUnitTreeBuilder.Build(administrativeUnits));
    }
}
#endregion