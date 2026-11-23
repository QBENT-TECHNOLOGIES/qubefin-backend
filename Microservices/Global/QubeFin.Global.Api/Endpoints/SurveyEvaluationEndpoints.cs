using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyEvaluations.Commands;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using QubeFin.Global.Application.SurveyEvaluations.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyEvaluationEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/evaluations", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetSurveyEvaluationsQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Committee Evaluations Of A Survey")
            .WithTags("SurveyEvaluations")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/evaluations", async (Guid id, EvaluationRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveMemberEvaluationCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Committee Member Evaluation")
            .WithTags("SurveyEvaluations")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/evaluations/final", async (Guid id, EvaluationRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveFinalEvaluationCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Committee Lead Final Decision")
            .WithTags("SurveyEvaluations")
            .RequireAuthorization();
        }
    }
}
