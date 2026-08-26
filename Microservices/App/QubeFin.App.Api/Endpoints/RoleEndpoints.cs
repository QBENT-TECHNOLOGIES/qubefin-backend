using MediatR;
using QubeFin.App.Application.Roles.Commands;
using QubeFin.App.Application.Roles.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using System.Security.Claims;
using System.Security.Principal;

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
        .WithSummary("Get All Roles")
        .WithTags("Roles")
        .RequireAuthorization();
        //.RequireAuthorization("Permission:Users.View")

        app.MapGet("roles/search", async (ISender sender, string? searchText, string sortOn, string sortDirection, int pageIndex, int pageSize) =>
        {
            var result = await sender.Send(new GetRolesBySearchQuery(searchText, sortOn, sortDirection, pageIndex, pageSize));
            return result.ToHttpResult();
        })
        .WithSummary("Search Roles by Free Text")
        .WithTags("Roles")
        .RequireAuthorization();

        app.MapGet("roles/{id}", async (ISender sender, Guid id) =>
        {
            var result = await sender.Send(new GetRoleByIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Role by Id")
        .WithTags("Roles")
        .RequireAuthorization();

        app.MapPost("roles/create", async (ISender sender, ClaimsPrincipal principal, CreateRoleCommand command) =>
        {
            Guid userId = principal.Identity.GetUserId();
            command = command with { UserId = userId };
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Create Role")
        .WithTags("Roles")
        .RequireAuthorization();

        app.MapPost("roles/update/{id}", async (ISender sender, ClaimsPrincipal principal, Guid id, UpdateRoleCommand command) =>
        {
            Guid userId = principal.Identity.GetUserId();
            command = command with { UserId = userId };
            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithSummary("Update Role")
        .WithTags("Roles")
        .RequireAuthorization();
    }
}
