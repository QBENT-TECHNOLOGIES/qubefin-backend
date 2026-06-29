using MediatR;
using QubeFin.App.Application.Users.Commands;
using QubeFin.App.Application.Users.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;

namespace QubeFin.App.Api.Endpoints;

public class UserEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (ISender sender) =>
        {
            var users = await sender.Send(new GetUsersQuery());
            return Results.Ok(users.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Users");

        app.MapGet("users/{id:guid}", async (Guid id, ISender sender) =>
        {
            var user = await sender.Send(new GetUserByIdQuery(id));
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get User By Id");

        app.MapPost("users", async (CreateUserCommand command, ISender sender) =>
        {
            await sender.Send(command);

            return Results.Ok();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create User");

        app.MapPost("register-mfa", async (RegisterMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            if (result.IsFailed)
            {
                if (result.Errors[0] is RecordNotFoundError)
                {
                    return Results.NotFound(result.Errors[0]);
                }
                if (result.Errors[0] is ValidationError)
                {
                    return Results.BadRequest(result.Errors[0]);
                }
            }

            return Results.Ok(result.Value);
        });

        app.MapPost("enable-mfa", async (EnableMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            if (result.IsFailed)
            {
                if (result.Errors[0] is RecordNotFoundError)
                {
                    return Results.NotFound(result.Errors[0]);
                }
                if (result.Errors[0] is ValidationError)
                {
                    return Results.BadRequest(result.Errors[0]);
                }
            }

            return Results.Ok(result.Value);
        });
    }
}
