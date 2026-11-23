using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyApprovals.Commands;
using QubeFin.Global.Application.SurveyApprovals.Queries;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyApprovalEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/approval", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetSurveyApprovalQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Managing Director Approval Of A Survey")
            .WithTags("SurveyApprovals")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/approval", async (Guid id, EvaluationRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveSurveyApprovalCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Managing Director Approves, Rejects Or Rechecks A Survey")
            .WithTags("SurveyApprovals")
            .RequireAuthorization();
        }
    }
}
