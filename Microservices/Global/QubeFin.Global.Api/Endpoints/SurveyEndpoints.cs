using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyUnit.Commands;
using QubeFin.Global.Application.SurveyUnit.Models;
using QubeFin.Global.Application.SurveyUnit.Queries;
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
            })
            .WithSummary("Search All Surveys")
            .WithTags("Surveys")
            .RequireAuthorization();

            app.MapGet("surveys/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetSurveyByIdQuery(id, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Survey By Id")
            .WithTags("Surveys")
            .RequireAuthorization();

            app.MapPost("surveys", async (ClaimsPrincipal principal, SurveyRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();

                var command = new CreateSurveyCommand(request, userId);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithSummary("Create Survey")
            .WithTags("Surveys")
            .RequireAuthorization();

            app.MapPut("surveys", async (ClaimsPrincipal principal, SurveyRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();

                var command = new UpdateSurveyCommand(request, userId);
                var result = await sender.Send(command);
                return result.ToHttpResult();
            })
            .WithSummary("Update Survey")
            .WithTags("Surveys")
            .RequireAuthorization();
        }
    }
}
