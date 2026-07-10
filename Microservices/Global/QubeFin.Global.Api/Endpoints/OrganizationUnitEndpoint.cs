using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Global.Application.OrganizationUnis.Commands;
using QubeFin.Global.Application.OrganizationUnits.Commands;
using QubeFin.Global.Application.OrganizationUnits.Type.Queries;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QubeFin.Global.Api.Endpoints
{
    public class OrganizationUnitEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("organization-unit-types", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllOrganizationUnitTypeQuery(), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get all organization unit types")
              .WithDescription("Retrieves a list of all organization unit types in the system.")
              .WithTags("Organization Unit Types");
            app.MapPost("organization-unit", async(CreateOrganizationUnitCommand command, ISender sender, ClaimsPrincipal principal) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();
                command = command with { createdBy = userId };
                var result = await sender.Send(command);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Create Organization Unit")
              .WithDescription("Create new organization unit in the system.")
              .WithTags("Organization Units");
            app.MapPut("organization-unit/{id}", async(Guid id, UpdateOrganizationCommand command, ISender sender, ClaimsPrincipal principal) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();
                command = command with { lastModifiedBy = userId };
                var result = await sender.Send(command);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Update Organization Unit")
              .WithDescription("Update existing organization unit in the system.")
              .WithTags("Organization Units");
        }
    }
}
