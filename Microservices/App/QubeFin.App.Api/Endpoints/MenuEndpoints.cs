using MediatR;
using QubeFin.App.Application.Menus.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Persistence.Models.App;

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
                return Results.StatusCode(500);
            }
            return Results.Ok(result.Value);
        })
            .WithSummary("Get Menu Tree");

        app.MapGet("menus/{id:guid}", async (Guid id, ISender sender) =>
        {
            var menu = await sender.Send(new GetMenuByIdQuery(id));
            return Results.Ok(menu.Value);
        })
            .WithSummary("Get Menu By Id");

    }
}
