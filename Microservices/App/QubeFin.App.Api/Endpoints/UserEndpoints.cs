using MediatR;
using QubeFin.App.Application.Users.Commands;
using QubeFin.Core.Endpoint;

namespace QubeFin.App.Api.Endpoints;

public class UserEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users", async (CreateUserCommand command, ISender sender) =>
        {
            await sender.Send(command);

            return Results.Ok();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create User"); ;
    }
}
