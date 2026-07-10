using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.OrganizationUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class OrganizationUnitTypeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("organization-unit-types", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOrganizationUnitTypesQuery(), cancellationToken);
            if (result.IsFailed)
            {
                return Results.Problem("Failed to retrieve Organization Unit tree");
            }
            return Results.Ok(result.Value);
        })
        .WithName("GetOrganizationUnitTree")
        .WithSummary("Get Organization Unit hierarchy")
        .WithDescription("Returns the complete hierarchical tree of all Organization Units.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
