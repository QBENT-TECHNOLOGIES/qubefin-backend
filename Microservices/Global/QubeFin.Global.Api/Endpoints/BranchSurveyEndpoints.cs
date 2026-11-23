using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.BranchSurveys.Commands;
using QubeFin.Global.Application.BranchSurveys.Models;
using QubeFin.Global.Application.BranchSurveys.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class BranchSurveyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/branch-form", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetBranchSurveyFormQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Branch Survey Form")
            .WithTags("BranchSurveys")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/branch-form", async (Guid id, BranchSurveyForm form, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveBranchSurveyDraftCommand(id, form, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Branch Survey Form As Draft")
            .WithTags("BranchSurveys")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/submit", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SubmitBranchSurveyCommand(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Submit Branch Survey To The Survey Committee")
            .WithTags("BranchSurveys")
            .RequireAuthorization();
        }
    }
}
