using FluentResults;
using MediatR;
using QubeFin.App.Application.Roles.Queries;
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
            var result = await sender.Send(new GetUsersQuery());
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get All Users");

        app.MapGet("users/search", async (ISender sender, string? searchText, string sortOn, string sortDirection, int pageIndex, int pageSize) =>
        {
            var result = await sender.Send(new GetUsersBySearchQuery(searchText, sortOn, sortDirection, pageIndex, pageSize));
            return result.ToHttpResult();
        })
        .WithSummary("Search Users by User Name or Employee Name");

        app.MapGet("users/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get User By Id");

        app.MapPost("users", async (CreateUserCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create User");

        app.MapPost("register-mfa", async (RegisterMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            return result.ToHttpResult();
        });

        app.MapPost("enable-mfa", async (EnableMfaCommand request, ISender sender, IPublisher publisher) =>
        {
            var result = await sender.Send(request);
            return result.ToHttpResult();
        });
    }
}
