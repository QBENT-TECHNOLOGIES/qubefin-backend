using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Lookups.Queries;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class LookupEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("me", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetTrackingViewerQuery(userId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get the signed-in tracking user")
        .WithDescription("Returns the signed-in user's employee id, name, tracking role, unit, branches in scope and whether they can create jobs.")
        .WithTags("Lookups")
        .RequireAuthorization();

        app.MapGet("organization-units", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetTrackingBranchesQuery(userId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get Organization Units in the user's scope")
        .WithDescription("Returns the branches the signed-in user may see (every branch for an Admin), sorted by name, with area, division, location and operating radius.")
        .WithTags("Lookups")
        .RequireAuthorization();

        app.MapGet("field-officers", async (Guid? organizationUnitId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetFieldOfficersQuery(userId, organizationUnitId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get field officers the user may monitor")
        .WithDescription("Returns everyone in a lower role inside the user's scope (plus, for a Branch Manager, the Assistant Branch Manager of their branch), with canAssign telling who may be given work. Optional organizationUnitId keeps only people who may work in that branch.")
        .WithTags("Lookups")
        .RequireAuthorization();
    }
}
