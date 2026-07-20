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

        app.MapGet("police-stations", async (ISender sender) =>
        {
            var user = await sender.Send(new GetPoliceStationsQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Police Station");

        app.MapGet("post-office", async (ISender sender) =>
        {
            var user = await sender.Send(new GetPostOfficesQuery());
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Post Office");
    }
}
