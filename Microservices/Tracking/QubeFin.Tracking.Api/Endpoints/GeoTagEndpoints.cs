using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Application.GeoTags.Commands;
using QubeFin.Tracking.Application.GeoTags.Queries;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class GeoTagEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("geo-tags/points", async (Guid organizationUnitId, string? kinds, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetGeoTagPointsQuery(userId, organizationUnitId, kinds), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get geo-tagged places of a branch")
        .WithDescription("Returns every geo-tagged group, member and OD member of one branch for the task picker, and how many of each kind have no location yet. kinds is comma-separated, e.g. GroupCentre,Member,OdMember.")
        .WithTags("Geo Tags")
        .RequireAuthorization();

        app.MapPost("geo-tags/search", async (GeoTagSearchRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetGeoTagsQuery(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search geo-tagged places of one kind")
        .WithDescription("Paged list of places of one kind (Branch, GroupCentre, Member, OdMember or EmployeeHome) in the user's branches, with and without a location. Everything is in the body: kind, searchText, organizationUnitId, onlyUntagged, sortOn (name | code | branchName | taggedAt), sortDirection (asc | desc), pageIndex (0-based), pageSize. Employee homes are for an Admin only and list active employees (no separation date).")
        .WithTags("Geo Tags")
        .RequireAuthorization();

        app.MapPut("geo-tags/{id:guid}", async (Guid id, UpdateGeoTagRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateGeoTagCommand(userId, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Set or change a location")
        .WithDescription("Sets or changes the location of a branch, group, member or employee home (id = the place's id, as refId in GET geo-tags), with a radius and a reason.")
        .WithTags("Geo Tags")
        .RequireAuthorization();
    }
}
