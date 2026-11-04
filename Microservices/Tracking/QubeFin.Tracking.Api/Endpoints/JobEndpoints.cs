using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Jobs.Models;
using QubeFin.Tracking.Application.Jobs.Queries;
using System.Security.Claims;

namespace QubeFin.Tracking.Api.Endpoints;

public class JobEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("jobs/search", async (JobSearchRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new SearchJobsQuery(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search jobs")
        .WithDescription("Returns one page of the jobs the signed-in user may see, with filters, sorting and the status counts for the top cards.")
        .WithTags("Jobs")
        .RequireAuthorization();
    }
}
