using Amazon.Auth.AccessControlPolicy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Commands;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Application.InterviewProcess.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class CandidateEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("candidates", async ([AsParameters] CandidateSearchParam searchParam, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidatesQuery(searchParam), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search interview candidates")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapGet("candidates/{id:guid}", async (Guid id, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetCandidateByIdQuery(id, empId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get interview candidate by ID")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapPost("candidates", async ([FromBody] CandidateCreateUpdateDto request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new CreateCandidateCommand(request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Create interview candidate")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapPut("candidates/{id:guid}", async (Guid id, [FromBody] CandidateCreateUpdateDto request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateCandidateCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Update interview candidate")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapPost("candidates/{id:guid}/letter-status", async (Guid id, [FromBody] CandidateLetterStatusRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateCandidateLetterStatusCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Update one candidate letter-received flag (send exactly one per call) and send the letter email")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapPost("candidates/{id:guid}/interview-upload", async (Guid id, [FromForm] CandidateInterviewUploadRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UploadCandidateInterviewFormatCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Upload the candidate's filled written-interview/personality form")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapGet("candidates/search", async (string searchText, int maxResults, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SearchCandidatesByTextQuery(searchText, maxResults), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Search candidates by text (returns Name (Ref No))")
        .WithTags("Candidates")
        .RequireAuthorization();

        app.MapGet("candidate-verifications/{candidateId:guid}", async (Guid candidateId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateVerificationQuery(candidateId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get candidate verification details")
        .WithTags("Candidate Verification")
        .RequireAuthorization();
    }
}
