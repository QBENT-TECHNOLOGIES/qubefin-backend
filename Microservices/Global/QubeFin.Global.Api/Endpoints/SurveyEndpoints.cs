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
            app.MapPost("surveys/search", async (SurveySearchParam searchParam, ISender sender) =>
            {
                var searchResults = await sender.Send(new GetSurveyBySearchQuery(searchParam));
                return Results.Ok(searchResults);
            }).WithSummary("Search All Surveys");

            app.MapGet("surveys/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetSurveyByIdQuery(id));
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

                return Results.Ok(result);
            }).WithSummary("Update Survey");
        }
    }
}
