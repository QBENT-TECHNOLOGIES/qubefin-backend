using MediatR;
using QubeFin.App.Application.Roles.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

namespace QubeFin.App.Api.Endpoints;

public class RoleEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("roles", async (ISender sender) =>
        {
            var result = await sender.Send(new GetRolesQuery());
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Roles");
    }
}
