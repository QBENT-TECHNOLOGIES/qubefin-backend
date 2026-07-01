using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

#region --- QUERY ---
public record GetAdministrativeUnitTreeQuery : IRequest<Result<List<AdministrativeUnitTree>>>;
#endregion

#region --- HANDLER ---
internal sealed class GetAdministrativeUnitTreeQueryHandler(IAdministrativeUnitRepository administrativeUnitRepository)
    : IRequestHandler<GetAdministrativeUnitTreeQuery, Result<List<AdministrativeUnitTree>>>
{
    public async Task<Result<List<AdministrativeUnitTree>>> Handle(GetAdministrativeUnitTreeQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnits = await administrativeUnitRepository.GetAllAsync(cancellationToken);
        return Result.Ok(AdministrativeUnitTreeBuilder.Build(administrativeUnits));
    }
}
#endregion