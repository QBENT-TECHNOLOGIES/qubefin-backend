using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Api.Requests;
using QubeFin.Global.Application.OrganizationUnis.Commands;
using QubeFin.Global.Application.OrganizationUnits.Commands;
using QubeFin.Global.Application.OrganizationUnits.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints;

public class OrganizationUnitEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("organization-units/{id:guid}", async (ISender sender, [FromRoute] Guid id) =>
        {
            var result = await sender.Send(new GetOrganizationUnitByIdQuery(id));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Organization Unit By Id").WithTags("OrganizationUnits");

        app.MapGet("organization-units/children/", async (ISender sender, Guid? id) =>
        {
            var result = await sender.Send(new GetOrganizationChildUnitsQuery(id));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.View")
        .WithSummary("Get Organization Units By Parent Id").WithTags("OrganizationUnits");

        app.MapGet("organization-units/tree", async (ISender sender) =>
        {
            var result = await sender.Send(new GetOrganizationUnitTreeQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get Organization Unit Tree").WithTags("OrganizationUnits");

        app.MapPost("organization-units", async (ISender sender, ClaimsPrincipal principal, [FromBody] OrganizationUnitRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var result = await sender.Send(new CreateOrganizationUnitCommand(request.OrganizationUnitTypeId, request.Name, request.Codeval, request.ParentId, 
                request.Latitude, request.Longitude, request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, userId));
            return result.ToHttpResult();
        })
        //.RequireAuthorization("Permission:Users.Add")
        .WithSummary("Create Organization Unit").WithTags("OrganizationUnits");

        app.MapPut("organization-units/{id:guid}", async (ISender sender, ClaimsPrincipal principal, [FromRoute] Guid id, [FromBody] OrganizationUnitRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();

            var result = await sender.Send(new UpdateOrganizationUnitCommand(id, request.OrganizationUnitTypeId, request.Name, request.Codeval, request.Latitude, request.Longitude, 
                request.AttendanceInTime, request.AttendanceOutTime, request.CheckRadiusInMeter, request.ParentId, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Update Organization Unit").WithTags("OrganizationUnits");
    }
}
