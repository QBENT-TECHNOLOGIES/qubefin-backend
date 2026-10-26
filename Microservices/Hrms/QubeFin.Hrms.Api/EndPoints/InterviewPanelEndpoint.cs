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
        #region INTERVIEW PANEL CREATION & UPDATION

        app.MapPost("interview-panels/schedule", async ([FromForm] InterviewPanelScheduleRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new ScheduleInterviewPanelCommand(request.CandidateId, request.ToPanelists(), principal.Identity.GetUserId(), request.File), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Schedule interview panel for a candidate and email the panelists")
        .WithTags("Interview Panels")
        .RequireAuthorization();

        app.MapPost("interview-panels/{candidateId:guid}/panelists", async (Guid candidateId, [FromForm] InterviewPanelScheduleRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new AddInterviewPanelistsCommand(candidateId, request.ToPanelists(), principal.Identity.GetUserId(), request.File), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Add panelist(s) to a candidate's existing interview panel and email them")
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

        app.MapGet("interview-panels/candidate/{candidateId:guid}", async (Guid candidateId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateInterviewPanelsQuery(candidateId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get interview panels for a candidate")
        .WithTags("Interview Panels")
        .RequireAuthorization();
        #endregion


        #region INTERVIEWER (Interview page)

        // The caller's own interviews only - EmployeeId always comes from the claims.
        app.MapPost("interviews/filter", async ([FromBody] InterviewerCandidateSearchParam searchParam, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new GetInterviewerCandidatesQuery(searchParam, principal.Identity.GetEmployeeId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("The signed-in interviewer's interviews")
        .WithTags("Interviews")
        .RequireAuthorization();

        app.MapPost("interviews/acknowledge", async ([FromBody] AcknowledgeInterviewsRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new AcknowledgeInterviewsCommand(request.CandidateIds ?? [], principal.Identity.GetEmployeeId(), principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Acknowledge one or more of the signed-in interviewer's interviews in one call")
        .WithTags("Interviews")
        .RequireAuthorization();

        app.MapPost("interviews/attendance", async ([FromBody] MarkCandidateAttendanceRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new MarkCandidateAttendanceCommand(request.CandidateId, request.IsPresent, request.Remarks, principal.Identity.GetEmployeeId(), principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Start Assessment: record whether the candidate attended (Present / Absent with remarks)")
        .WithTags("Interviews")
        .RequireAuthorization();

        #endregion

        #region INTERVIEW ASSESSMENT
        app.MapGet("interview-panels/assessment/{candidateId:guid}/{employeeId:guid}", async (Guid candidateId, Guid employeeId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetInterviewAssessmentByCandidateAndEmployeeQuery(candidateId, employeeId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get a panelist's interview assessment by candidate and employee")
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
        .WithSummary("Save interview assessment as draft")
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
        #endregion

        #region HR ASSESSMESNT

        app.MapGet("hr-assessment/{candidateId:guid}", async (Guid candidateId, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetHrAssessmentFormQuery(candidateId, hrEmployeeId), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Open the HR Assessment form for a candidate (averages of the submitted panelists' ratings + HR's own draft, if any)").WithTags("HR Assessment").RequireAuthorization();

        app.MapPost("hr-assessment/{candidateId:guid}/draft", async (Guid candidateId, [FromBody] HrAssessmentDecisionDto decision, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new SaveHrAssessmentDraftCommand(candidateId, hrEmployeeId, decision, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Save the HR Assessment as a draft").WithTags("HR Assessment").RequireAuthorization();

        app.MapPost("hr-assessment/{candidateId:guid}/submit", async (Guid candidateId, [FromBody] HrAssessmentDecisionDto decision, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new SubmitHrAssessmentCommand(candidateId, hrEmployeeId, decision, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Submit (finalize) the HR Assessment - requires every panelist to have submitted first").WithTags("HR Assessment").RequireAuthorization();
        #endregion
    }
}
