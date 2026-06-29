using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.AdministrativeUnits.Commands;
using QubeFin.Global.Application.AdministrativeUnits.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class AdministrativeUnitEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("administrative-units/{id:guid}", async (Guid id, ISender sender) =>
        {
            var user = await sender.Send(new GetAdministrativeUnitByIdQuery(id));
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Administrative Unit By Id");

        app.MapGet("administrative-units/tree", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAdministrativeUnitsQuery(), cancellationToken);
            if (result.IsFailed)
            {
                return Results.StatusCode(500);
            }
            return Results.Ok(result.Value);
        });

        app.MapPost("administrative-units", async (CreateAdministrativeUnitCommand command, ISender sender) =>
        {
            await sender.Send(command);

            return Results.Ok();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create Administrative Unit"); ;
    }
}
