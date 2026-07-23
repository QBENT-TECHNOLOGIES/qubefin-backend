using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.App.Api.Requests;
using QubeFin.App.Application.Menus.Commands;
using QubeFin.App.Application.Menus.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using System.Security.Claims;

namespace QubeFin.App.Api.Endpoints;

public class MenuEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("menus/tree", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetMenuTreeQuery(), cancellationToken);
            if (result.IsFailed)
            {
                return Results.Problem("Failed to retrieve menu tree.");
            }
            return Results.Ok(result.Value);
        })
        .WithName("GetMenuTree")
        .WithSummary("Get menu hierarchy")
        .WithDescription("Returns the complete hierarchical tree of all application menus.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("menus/parent-only", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetParentMenusQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .WithName("GetParentMenus")
        .WithSummary("Get parent menus")
        .WithDescription("Returns the parent menus only.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("menus/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMenuByIdQuery(id));
            if (result.IsFailed)
            {
                return Results.NotFound();
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetMenuById")
        .WithSummary("Get menu by ID")
        .WithDescription("Retrieves a single menu using its unique identifier.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("menus/{target}", async (ISender sender, [FromRoute] string target) =>
        {
            var result = await sender.Send(new GetMenuByTargetQuery(Uri.UnescapeDataString(target)));
            return result.ToHttpResult();
        })
        .WithName("GetMenuByTarget")
        .WithSummary("Get menu by Target Path")
        .WithDescription("Retrieves a single menu using its target path.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPost("menus", async (ClaimsPrincipal principal, ISender sender, [FromBody] MenuRequest menu) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            await sender.Send(new CreateMenuCommand(menu.Name, menu.Icon, menu.Target, menu.ParentId, userId));
            return Results.Ok();
        })
       .WithSummary("Create Menu");

        app.MapPut("menus/{id:guid}", async (ClaimsPrincipal principal, ISender sender, [FromRoute] Guid id, [FromBody] MenuRequest menu) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            await sender.Send(new UpdateMenuCommand(id, menu.Name, menu.Icon, menu.Target, menu.ParentId, userId));
            return Results.Ok();
        })
        .WithSummary("Update Menu");
    }
}
