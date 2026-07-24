using Azure.Core;
using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit.Commands;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Application.SurveyUnit.Queries;
using QubeFin.Persistence.Models.App;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("surveys/search", async (SurveySearchParam searchParam, ClaimsPrincipal principal, ISender sender) =>
            {
                searchParam.EmployeeId = principal.Identity.GetEmployeeId();
                searchParam.UserId = principal.Identity.GetUserId();
                var searchResults = await sender.Send(new GetSurveyBySearchQuery(searchParam));
                return Results.Ok(searchResults);
            }).WithSummary("Search All Surveys");

            app.MapGet("surveys/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetSurveyByIdQuery(id, employeeId));
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok(result.Value);
            }).WithSummary("Get Survey By Id");

            app.MapPost("surveys", async (ClaimsPrincipal principal, SurveyRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();

                var command = new CreateSurveyCommand(request, userId);
                var result = await sender.Send(command);
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok();
            }).WithSummary("Create Survey");

            app.MapPut("surveys", async (ClaimsPrincipal principal, SurveyRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();

                var command = new UpdateSurveyCommand(request, userId);
                var result = await sender.Send(command);
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok();
            }).WithSummary("Update Survey");

            app.MapGet("surveys/branch/{surveyId:guid}", async (Guid surveyId, ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetBranchSurveyById(surveyId, employeeId));
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok(result.Value);
            }).WithSummary("Get Branch Survey By Id");

            app.MapPost("surveys/branch", async (ClaimsPrincipal principal, BranchSurveyRequest request, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var command = new CreateBranchSurveyCommand(request, userId);
                var result = await sender.Send(command);
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok();
            }).WithSummary("Create Branch Survey");

            app.MapPut("surveys/branch", async (ClaimsPrincipal principal, BranchSurveyRequest request, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var command = new UpdateBranchSurveyCommand(request, userId);
                var result = await sender.Send(command);
                if (result.IsFailed)
                {
                    if (result.Errors[0] is RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok();
            }).WithSummary("Update Branch Survey");
        }
    }
}
