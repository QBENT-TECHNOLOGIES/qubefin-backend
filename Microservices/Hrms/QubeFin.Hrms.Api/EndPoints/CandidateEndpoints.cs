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
        #region CANDIDATE
        app.MapPost("candidates/filter", async ([FromBody] CandidateSearchParam searchParam, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetCandidatesQuery(searchParam, empId), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Search interview candidates").WithTags("Candidates").RequireAuthorization();

        app.MapGet("candidates/search", async (string searchText, int maxResults, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SearchCandidatesByTextQuery(searchText, maxResults), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Search candidates by text (returns Name (Ref No))").WithTags("Candidates").RequireAuthorization();

        app.MapGet("candidates/{id:guid}", async (Guid id, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetCandidateByIdQuery(id, empId), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Get interview candidate by ID").WithTags("Candidates").RequireAuthorization();

        app.MapPost("candidates", async ([FromBody] CandidateCreateUpdateDto request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new CreateCandidateCommand(request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Create interview candidate").WithTags("Candidates").RequireAuthorization();

        app.MapPut("candidates/{id:guid}", async (Guid id, [FromBody] CandidateCreateUpdateDto request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateCandidateCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Update interview candidate").WithTags("Candidates").RequireAuthorization();

        app.MapPost("candidates/{id:guid}/interview-mode", async (Guid id, [FromBody] CandidateInterviewModeUpdateRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateCandidateInterviewModeCommand(id, request.InterviewMode, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Set whether the candidate's interview was Online or Offline").WithTags("Candidates").RequireAuthorization();

        #endregion

        #region SEND MAIL & RECEIVE FLAG UPDATE

        // Multipart, not JSON: the caller posts the rendered letter PDF as `File` and it is what gets
        // attached to the mail.
        app.MapPost("candidates/{id:guid}/send-letter", async (Guid id, [FromForm] CandidateLetterStatusRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new SendLetterToCandidateCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).DisableAntiforgery().WithSummary("Send letter to candidate").WithTags("Candidates").RequireAuthorization();

        app.MapPost("candidates/{id:guid}/letter-status", async (Guid id, [FromBody] CandidateLetterStatusRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UpdateCandidateLetterStatusCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Update one candidate letter-received flag (send exactly one per call) and send the letter email").WithTags("Candidates").RequireAuthorization();
        #endregion

        #region UPLOAD INTERVIEW RELATED DOC

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

        app.MapPost("candidates/{id:guid}/joining-letter-upload", async (Guid id, [FromForm] CandidateJoiningLetterUploadRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new UploadJoiningLetterCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Upload the candidate's signed/returned joining letter")
        .WithTags("Candidates")
        .RequireAuthorization();
        #endregion

        #region CANDIDATE VERIFICATION

        app.MapGet("candidate-verifications/{candidateId:guid}", async (Guid candidateId, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateVerificationQuery(candidateId), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Get candidate verification details").WithTags("Candidate Verification").RequireAuthorization();

        app.MapPut("candidate-verifications/{candidateId:guid}", async (Guid candidateId, [FromBody] CandidateVerificationUpdateRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var command = new UpdateCandidateVerificationCommand(
                candidateId,
                request.IsAadharValidated,
                request.IsVoterValited,
                request.IsPanValidated,
                request.IsMobileValidated,
                request.IsUanVerified,
                request.IsCreditBureauChecked,
                request.CreditBureauReportLink,
                principal.Identity.GetUserId());

            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Update candidate verification checks (all flags sent together)").WithTags("Candidate Verification").RequireAuthorization();
        #endregion

        app.MapGet("candidates/{id:guid}/joining-letter-status", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateJoiningLetterStatusQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Whether the candidate's signed joining letter has been uploaded, and its download URL if so")
        .WithTags("Candidates")
        .RequireAuthorization();

        #region JOINING INFO
        // Joining information captured once the candidate has joined. The Personal step creates the employee and
        // links it to the candidate (Tbl_Employee.CandidateId); every other step then uses the employee APIs.

        app.MapGet("candidates/{id:guid}/joining", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateJoiningInfoQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Get the employee created from the candidate and the candidate details the joining steps prefill").WithTags("Candidate Joining").RequireAuthorization();

        app.MapGet("candidates/{id:guid}/joining/personal", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCandidateJoiningPersonalQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).WithSummary("Get joining personal info (prefilled from the candidate until the employee is created)").WithTags("Candidate Joining").RequireAuthorization();

        app.MapPost("candidates/{id:guid}/joining/personal", async (Guid id, [FromForm] CandidateJoiningPersonalRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(new SaveCandidateJoiningPersonalCommand(id, request, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .DisableAntiforgery()
        .WithSummary("Save joining personal info with photo and signature - creates and links the employee on first save")
        .WithTags("Candidate Joining")
        .RequireAuthorization();
        #endregion
    }
}
