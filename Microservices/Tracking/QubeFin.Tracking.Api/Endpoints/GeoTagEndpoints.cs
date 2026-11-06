using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.GeoTags.Queries;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class GeoTagEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("geo-tags/points", async (Guid organisationUnitId, string? kinds, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetGeoTagPointsQuery(userId, organisationUnitId, kinds), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get geo-tagged places of a branch")
        .WithDescription("Returns every geo-tagged group, member and OD member of one branch for the task picker, and how many of each kind have no location yet. kinds is comma-separated, e.g. GroupCentre,Member,OdMember.")
        .WithTags("Geo Tags")
        .RequireAuthorization();

        app.MapGet("geo-tags", async (GeoTagKind? kind, string? searchText, Guid? organisationUnitId, bool? onlyUntagged, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetGeoTagsQuery(userId, kind, searchText, organisationUnitId, onlyUntagged ?? false), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("List geo-tagged places of one kind")
        .WithDescription("Returns places of one kind (Branch, GroupCentre, Member, OdMember or EmployeeHome) in the user's branches, with and without a location. onlyUntagged = true returns only places with no location yet. Employee homes are for an Admin only.")
        .WithTags("Geo Tags")
        .RequireAuthorization();
    }
}
