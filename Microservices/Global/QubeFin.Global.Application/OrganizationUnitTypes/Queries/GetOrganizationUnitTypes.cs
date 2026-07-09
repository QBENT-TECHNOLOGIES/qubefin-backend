using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnitTypes.Queries;

#region --- QUERY ---
public record GetOrganizationUnitTypesQuery : IRequest<Result<List<GetOrganizationUnitTypesResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetOrganizationUnitTypesResponse(Guid Id, string Name, int LevelNo);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitTypesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitTypesQuery, Result<List<GetOrganizationUnitTypesResponse>>>
{
    public async Task<Result<List<GetOrganizationUnitTypesResponse>>> Handle(GetOrganizationUnitTypesQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnitTypes = await context
            .TblOrganizationUnitTypes
            .OrderBy(a => a.LevelNo)
            .Select(a => new GetOrganizationUnitTypesResponse(a.Id, a.Name, a.LevelNo))
            .ToListAsync(cancellationToken);

        return Result.Ok(administrativeUnitTypes);
    }
}
#endregion
