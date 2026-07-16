using MediatR;
using QubeFin.App.Application.Roles.Queries;
using QubeFin.Core.Endpoint;

namespace QubeFin.App.Api.Endpoints;

public class RoleEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("roles", async (ISender sender) =>
        {
            var roles = await sender.Send(new GetRolesQuery());
            return Results.Ok(roles.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Roles");
    }
}
