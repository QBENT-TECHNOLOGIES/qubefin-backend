using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.OrganizationUnitTypes.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class UtilityEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("utilities", async (ISender sender) =>
        {
            var user = await sender.Send(new GetUtilityQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Utility");

        
    }
}
