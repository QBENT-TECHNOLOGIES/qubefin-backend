using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Global.Application.AdministrativeUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class AdministrativeUnitTypeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("administrative-unit-types", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAdministrativeUnitTypesQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization();
    }
}
