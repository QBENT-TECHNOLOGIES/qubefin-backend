using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.AdministrativeUnitTypes.Queries;

#region --- QUERY ---
public record GetAdministrativeUnitTypesQuery : IRequest<Result<List<GetAdministrativeUnitTypesResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetAdministrativeUnitTypesResponse(Guid Id, string Name, string Icon, int LevelNo, string Category);
#endregion

#region --- HANDLER ---
internal sealed class GetAdministrativeUnitTypesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAdministrativeUnitTypesQuery, Result<List<GetAdministrativeUnitTypesResponse>>>
{
    public async Task<Result<List<GetAdministrativeUnitTypesResponse>>> Handle(GetAdministrativeUnitTypesQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnitTypes = await context
            .TblAdministrativeUnitTypes
            .OrderBy(a => a.Category).ThenBy(a => a.LevelNo)
            .Select(a => new GetAdministrativeUnitTypesResponse(a.Id, a.Name, a.Icon, a.LevelNo, a.Category))
            .ToListAsync(cancellationToken);

        return Result.Ok(administrativeUnitTypes);
    }
}
#endregion