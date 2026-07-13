using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.OrganizationUnits.Queries;

namespace QubeFin.Global.Api.Endpoints;

public class OrganizationUnitEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("organization-units/{id:guid}", async (Guid id, ISender sender) =>
        {
            var user = await sender.Send(new GetOrganizationUnitByIdQuery(id));
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Organization Unit By Id");

        app.MapGet("organization-units/children/", async (Guid? id, ISender sender) =>
        {
            var user = await sender.Send(new GetOrganizationChildUnitsQuery(id));
            return Results.Ok(user.Value);
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Organization Units By Parent Id");

        app.MapGet("organization-units/tree", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOrganizationUnitTreeQuery(), cancellationToken);
            if (result.IsFailed)
            {
                return Results.StatusCode(500);
            }
            return Results.Ok(result.Value);
        });

        //app.MapPost("organization-units", async (CreateOrganizationUnitCommand command, ISender sender) =>
        //{
        //    await sender.Send(command);

        //    return Results.Ok();
        //})
        ////.RequireAuthorization("Permission:Users.Add")
        //.WithSummary("Create Organization Unit"); ;
    }
}
