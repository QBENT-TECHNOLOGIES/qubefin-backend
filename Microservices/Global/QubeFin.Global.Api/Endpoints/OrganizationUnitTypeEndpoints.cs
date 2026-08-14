using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Global.Application.OrganizationUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class OrganizationUnitTypeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("organization-unit-types", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOrganizationUnitTypesQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetOrganizationUnitTree")
        .WithSummary("Get Organization Unit hierarchy")
        .WithDescription("Returns the complete hierarchical tree of all Organization Units.")
        .WithTags("OrganizationUnitTypes")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
