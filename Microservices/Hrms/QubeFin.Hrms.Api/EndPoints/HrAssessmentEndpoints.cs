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

public class HrAssessmentEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("hr-assessment/{candidateId:guid}", async (Guid candidateId, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
        {
            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetHrAssessmentFormQuery(candidateId, hrEmployeeId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Open the HR Assessment form for a candidate (averages of the submitted panelists' ratings + HR's own draft, if any)")
        .WithTags("HR Assessment")
        .RequireAuthorization();

        app.MapPost("hr-assessment/{candidateId:guid}/draft", async (Guid candidateId, [FromBody] HrAssessmentDecisionDto decision, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new SaveHrAssessmentDraftCommand(candidateId, hrEmployeeId, decision, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Save the HR Assessment as a draft")
        .WithTags("HR Assessment")
        .RequireAuthorization();

        app.MapPost("hr-assessment/{candidateId:guid}/submit", async (Guid candidateId, [FromBody] HrAssessmentDecisionDto decision, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var hrEmployeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new SubmitHrAssessmentCommand(candidateId, hrEmployeeId, decision, principal.Identity.GetUserId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Submit (finalize) the HR Assessment - requires every panelist to have submitted first")
        .WithTags("HR Assessment")
        .RequireAuthorization();
    }
}
