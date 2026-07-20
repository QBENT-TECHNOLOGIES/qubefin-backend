using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnitTypes.Queries;

#region --- QUERY ---
public record GetPoliceStationsQuery : IRequest<Result<List<GetPoliceStationResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetPoliceStationResponse(Guid Id, string Name);
#endregion

#region --- HANDLER ---
internal sealed class GetPoliceStationsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetPoliceStationsQuery, Result<List<GetPoliceStationResponse>>>
{
    public async Task<Result<List<GetPoliceStationResponse>>> Handle(GetPoliceStationsQuery request, CancellationToken cancellationToken)
    {
        var utilities = await context
            .TblPoliceStations
            .OrderBy(a => a.Name)
            .Select(a => new GetPoliceStationResponse(a.Id, a.Name))
            .ToListAsync(cancellationToken);

        return Result.Ok(utilities);
    }
}
#endregion
