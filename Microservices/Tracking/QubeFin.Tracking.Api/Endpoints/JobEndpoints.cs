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

        app.MapGet("jobs/task-points", async (Guid? organisationUnitId, Guid? assigneeId, bool? pendingOnly, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetTaskPointsQuery(userId, organisationUnitId, assigneeId, pendingOnly ?? true), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get task locations of open jobs")
        .WithDescription("Returns the task locations of open jobs the signed-in user may see, for the duplicate check in the task picker. pendingOnly defaults to true; false also returns completed tasks.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapGet("jobs/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetJobByIdQuery(userId, id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get one job")
        .WithDescription("Returns one job with all its tasks (including cancelled, rejected and transferred ones) and what the signed-in user may do with it. Outside the user's scope returns 404.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapGet("jobs/{id:guid}/history", async (Guid id, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetJobHistoryQuery(userId, id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get a job's history")
        .WithDescription("Returns the job's audit trail, newest first, rebuilt from the job and task records. Outside the user's scope returns 404.")
        .WithTags("Jobs")
        .RequireAuthorization();
    }
}
