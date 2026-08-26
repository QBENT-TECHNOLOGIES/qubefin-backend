using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.App.Api.Requests;
using QubeFin.App.Application.Menus.Commands;
using QubeFin.App.Application.Menus.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.App;
using System.Security.Claims;

namespace QubeFin.App.Api.Endpoints;

public class MenuEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("menus/tree", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetMenuTreeQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetMenuTree")
        .WithSummary("Get menu hierarchy")
        .WithDescription("Returns the complete hierarchical tree of all application menus.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("menus/tree-by-user", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();

            var result = await sender.Send(new GetMenuTreeByUserQuery(employeeId), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetMenuTreeByUser")
        .WithSummary("Get menu hierarchy by user")
        .WithDescription("Returns the complete hierarchical tree of all application menus for the authenticated user.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("menus/parent-only", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetParentMenusQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetParentMenus")
        .WithSummary("Get parent menus")
        .WithDescription("Returns the parent menus only.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);


        app.MapGet("menus/parent-only-by-user", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();

            var result = await sender.Send(new GetParentMenusByUserQuery(employeeId), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetParentMenusByUser")
        .WithSummary("Get parent menus by user")
        .WithDescription("Returns the parent menus only for the authenticated user.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("menus/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMenuByIdQuery(id));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetMenuById")
        .WithSummary("Get menu by ID")
        .WithDescription("Retrieves a single menu using its unique identifier.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("menus/{target}", async (ISender sender, [FromRoute] string target) =>
        {
            var result = await sender.Send(new GetMenuByTargetQuery(Uri.UnescapeDataString(target)));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithName("GetMenuByTarget")
        .WithSummary("Get menu by Target Path")
        .WithDescription("Retrieves a single menu using its target path.")
        .WithTags("Menus")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("menus", async (ClaimsPrincipal principal, ISender sender, [FromBody] MenuRequest menu) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var permissions = menu.Permissions
                .Select(x => new PermissionAssigned { Id = x.Id })
            .ToList();

            var result = await sender.Send(new CreateMenuCommand(menu.Name, menu.Icon, menu.Target, menu.ParentId, userId, permissions));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithSummary("Create Menu")
        .WithTags("Menus");

        app.MapPut("menus/{id:guid}", async (ClaimsPrincipal principal, ISender sender, [FromRoute] Guid id, [FromBody] MenuRequest menu) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var permissions = menu.Permissions
                .Select(x => new PermissionAssigned { Id = x.Id })
            .ToList();

            var result = await sender.Send(new UpdateMenuCommand(id, menu.Name, menu.Icon, menu.Target, menu.ParentId, userId, permissions));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithSummary("Update Menu")
        .WithTags("Menus");
    }
}
