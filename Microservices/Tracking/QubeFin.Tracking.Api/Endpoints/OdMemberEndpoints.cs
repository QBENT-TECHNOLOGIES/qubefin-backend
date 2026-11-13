using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.OdMembers.Queries;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class OdMemberEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("od-members/search", async (OdMemberSearchRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new SearchOdMembersQuery(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search OD members")
        .WithDescription("Paged list of overdue members with their open loan, in the signed-in user's branches, with the total overdue amount over the filtered rows. Filters: branch, Credit Officer, group, DPD range, without a pending visit; searchText matches member name or code, loan no, group or Credit Officer. Each row has the latest OD follow-up and every pending one.")
        .WithTags("OD Members")
        .RequireAuthorization();
    }
}
