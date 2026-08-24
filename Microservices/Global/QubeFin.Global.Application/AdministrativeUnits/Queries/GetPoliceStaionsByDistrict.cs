using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

public record GetPoliceStationsByDistrictQuery(Guid disrtictId) : IRequest<Result<List<GetPoliceStationsByDistrictResponse>>>;
public record GetPoliceStationsByDistrictResponse(Guid Id, string Name);

internal sealed class GetPoliceStationsByDistrictHandler(IAdministrativeUnitRepository administrativeUnit) : IRequestHandler<GetPoliceStationsByDistrictQuery, Result<List<GetPoliceStationsByDistrictResponse>>>
{
    public async Task<Result<List<GetPoliceStationsByDistrictResponse>>> Handle(GetPoliceStationsByDistrictQuery request, CancellationToken cancellationToken)
    {
        var postoffices = await administrativeUnit.GetPoliceStationsByDistrictAsync(request.disrtictId);
        return Result.Ok(postoffices.Select(m => new GetPoliceStationsByDistrictResponse(m.Id, m.Name)).ToList());
    }
}
