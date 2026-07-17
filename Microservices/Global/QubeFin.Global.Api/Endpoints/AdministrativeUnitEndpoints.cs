using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Api.Requests;
using QubeFin.Global.Application.AdministrativeUnits.Commands;
using QubeFin.Global.Application.AdministrativeUnits.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints;

public class AdministrativeUnitEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("administrative-units/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrativeUnitByIdQuery(id));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Administrative Unit By Id");

        app.MapGet("administrative-units/children/", async (Guid? id, ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrativeChildUnitsQuery(id));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Administrative Units By Parent Id");

        app.MapGet("administrative-units/tree", async (ISender sender) =>
        {
            var result = await sender.Send(new GetAdministrativeUnitTreeQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get Administrative Units Tree"); ;

        app.MapPost("administrative-units", async (ISender sender, ClaimsPrincipal principal, [FromBody] AdministrativeUnitRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var result = await sender.Send(new CreateAdministrativeUnitCommand(request.AdministrativeUnitTypeId, request.Name, request.ParentId, userId));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create Administrative Unit");

        app.MapPut("administrative-units/{id:guid}", async (ISender sender, ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] AdministrativeUnitRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var result = await sender.Send(new UpdateAdministrativeUnitCommand(id, request.AdministrativeUnitTypeId, request.Name, request.ParentId, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Update Document Type"); ;
    }
}
