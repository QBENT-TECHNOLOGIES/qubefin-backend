using MediatR;
using QubeFin.App.Application.Permissions.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

namespace QubeFin.App.Api.Endpoints;

public class PermissionEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("permissions", async (ISender sender) =>
        {
            var result = await sender.Send(new GetPermissionsQuery());
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Permission Tokens");
    }
}
