using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.BcLists.Queries;
using QubeFin.Tracking.Application.GeoTags.Commands;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class BcEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("groups/search", async (GroupListRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetGroupListQuery(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search groups")
        .WithDescription("Paged list of BC groups the signed-in user may see (a Credit Officer: their own groups; BM / ABM: their branch; AM: their area; DM: their division; super admin: all), filtered by branch, Credit Officer and group name or code.")
        .WithTags("Groups & Members")
        .RequireAuthorization();

        app.MapPost("members/search", async (MemberListRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetMemberListQuery(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search members")
        .WithDescription("Paged list of BC members the signed-in user may see, filtered by branch, Credit Officer and group; searchText matches name, member code, Aadhaar / PAN / Voter no or mobile.")
        .WithTags("Groups & Members")
        .RequireAuthorization();

        app.MapPut("groups/{id:guid}/location", async (Guid id, [FromForm] UpdateBcLocationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateBcLocationCommand(userId, BcPlace.Group, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Update a group's location")
        .WithDescription("multipart/form-data with Photo, Latitude and Longitude. Saves the location and photo and records who tagged it.")
        .WithTags("Groups & Members")
        .RequireAuthorization();

        app.MapPut("members/{id:guid}/location", async (Guid id, [FromForm] UpdateBcLocationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new UpdateBcLocationCommand(userId, BcPlace.Member, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Update a member's location")
        .WithDescription("multipart/form-data with Photo, Latitude and Longitude. Saves the home location and photo and records who tagged it.")
        .WithTags("Groups & Members")
        .RequireAuthorization();
    }
}
