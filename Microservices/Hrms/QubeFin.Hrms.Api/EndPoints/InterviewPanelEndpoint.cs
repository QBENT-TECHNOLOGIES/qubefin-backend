using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Commands;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.EndPoints;

public class InterviewPanelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("interview-panels/candidate/{candidateId:guid}", async (Guid candidateId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateInterviewPanelsQuery(candidateId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get interview panels for a candidate")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/schedule", async ([FromBody] ScheduleInterviewPanelCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(command with { ScheduledBy = principal.Identity.GetUserId() }, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Schedule interview panel for a candidate")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/{candidateId:guid}/panelists", async (Guid candidateId, [FromBody] List<PanelistScheduleDto> panelists, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new AddInterviewPanelistsCommand(candidateId, panelists, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Add panelist(s) to a candidate's existing interview panel")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapDelete("interview-panels/{candidateId:guid}/panelists/{employeeId:guid}", async (Guid candidateId, Guid employeeId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new RemoveInterviewPanelistCommand(candidateId, employeeId, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Remove a panelist from a candidate's interview panel")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapGet("interview-panels/assessment/{candidateId:guid}/{employeeId:guid}", async (Guid candidateId, Guid employeeId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetInterviewAssessmentByCandidateAndEmployeeQuery(candidateId, employeeId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get a panelist's interview assessment by candidate and employee")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/{candidateId:guid}/acknowledge", async (Guid candidateId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new AcknowledgePanelInvitationCommand(candidateId, principal.Identity.GetEmployeeId(), principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Acknowledge interview panel invitation")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/{panelId:guid}/attendance", async (Guid panelId, [FromBody] bool isAttended, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new MarkPanelAttendanceCommand(panelId, isAttended, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Mark whether a panelist attended the interview")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/assessment", async ([FromBody] SubmitInterviewAssessmentCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(command with { EmployeeId = principal.Identity.GetEmployeeId(), SubmittedBy = principal.Identity.GetUserId() }, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Submit interview assessment")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/assessment/draft", async ([FromBody] SaveInterviewAssessmentDraftCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(command with { EmployeeId = principal.Identity.GetEmployeeId(), SavedBy = principal.Identity.GetUserId() }, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Save interview assessment as draft (marks the panelist as attended)")
        .WithTags("Interview Panels")
        .RequireAuthorization();
    }
}
