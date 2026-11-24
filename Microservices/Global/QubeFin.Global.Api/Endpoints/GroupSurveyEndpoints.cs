using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupSurveys.Commands;
using QubeFin.Global.Application.GroupSurveys.Models;
using QubeFin.Global.Application.GroupSurveys.Queries;
using QubeFin.Global.Application.SurveyEvaluations.Models;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class GroupSurveyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/group-form", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupSurveyFormQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Group Survey Form")
            .WithTags("GroupSurveys")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/group-form", async (Guid id, GroupSurveyForm form, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveGroupSurveyDraftCommand(id, form, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Group Survey Form As Draft")
            .WithTags("GroupSurveys")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/group-submit", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SubmitGroupSurveyCommand(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Submit Group Survey To The Head Of Branch")
            .WithTags("GroupSurveys")
            .RequireAuthorization();

            app.MapGet("surveys/{id:guid}/group-review", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupReviewQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Recommendation And Approval Of A Group Survey")
            .WithTags("GroupSurveys")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/group-recommendation", async (Guid id, GroupRecommendationRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveGroupRecommendationCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Head Of Branch Recommends A Group Survey Or Sends It Back")
            .WithTags("GroupSurveys")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/group-approval", async (Guid id, EvaluationRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveGroupApprovalCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Area Manager Approves, Rejects Or Rechecks A Group Survey")
            .WithTags("GroupSurveys")
            .RequireAuthorization();
        }
    }
}
