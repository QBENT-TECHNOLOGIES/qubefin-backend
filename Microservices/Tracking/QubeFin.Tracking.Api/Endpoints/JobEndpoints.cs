using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Tracking.Application.Jobs.Commands;
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

        app.MapPost("jobs", async (CreateJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CreateJobCommand(userId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Create a job")
        .WithDescription("Creates a job with its assignee and tasks. Tasks that are duplicates or at an untagged place are skipped with a reason; tasks pending with someone else at the same place come back as warnings.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPost("jobs/{id:guid}/tasks", async (Guid id, AddTasksRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new AddTasksCommand(userId, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Add tasks to a job")
        .WithDescription("Adds tasks to an open job (Assigned, InProgress or Delayed). The same rules as creating a job apply, checked against the job's assignee.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/transfer", async (Guid id, TransferJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new TransferJobCommand(userId, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Transfer a job")
        .WithDescription("Gives the whole job, or some pending tasks, to someone else. Completed tasks never move; a partial transfer creates a new linked job for the new person.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/extend", async (Guid id, ExtendJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new ExtendJobCommand(userId, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Extend a job's complete-by date")
        .WithDescription("Moves an open job's complete-by date later, with a reason. The new date must be later than the current one and not in the past.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/cancel", async (Guid id, CancelJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CancelJobCommand(userId, id, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Cancel a job")
        .WithDescription("Cancels an open job with a reason. Pending tasks become Cancelled; completed tasks stay on record.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/tasks/{taskId:guid}/cancel", async (Guid id, Guid taskId, CancelTaskRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CancelTaskCommand(userId, id, taskId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Cancel one task")
        .WithDescription("Cancels one pending task of a job with a reason. If no pending task is left and at least one is done, the job completes.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/tasks/{taskId:guid}/reject", async (Guid id, Guid taskId, RejectTaskRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new RejectTaskCommand(userId, id, taskId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Reject a self-added task")
        .WithDescription("Refuses a pending task the assignee added from the mobile app, with a reason. Tasks added on the web must be cancelled instead.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapPut("jobs/{id:guid}/tasks/{taskId:guid}/complete", async (Guid id, Guid taskId, [FromForm] CompleteTaskRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CompleteTaskCommand(userId, id, taskId, request), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Complete a task")
        .WithDescription("multipart/form-data with Photo, Latitude, Longitude and Remarks; OD follow-up tasks also send MemberAvailable, Paid, AmountCollected (when paid) and PtpDate (when not paid). Only the job's assignee can complete a pending task. The distance from the place is worked out and a mismatch is flagged. If it was the last pending task, the job completes.")
        .WithTags("Jobs")
        .RequireAuthorization();

        app.MapGet("jobs/open", async (Guid? organisationUnitId, Guid? assigneeId, string? searchText, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Unauthorized();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new GetOpenJobsQuery(userId, organisationUnitId, assigneeId, searchText), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get open jobs for adding tasks")
        .WithDescription("Open jobs (Assigned, InProgress, Delayed) in the signed-in user's branches (BM: branch, AM: area, DM: division, super admin: all), soonest complete-by first. organisationUnitId keeps jobs whose assignee covers that branch; assigneeId keeps one person's jobs; searchText matches job no, title or assignee.")
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
