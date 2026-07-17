using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Global.Application.Survey.Models;
using QubeFin.Global.Application.Survey.Queries;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("surveys", async (SurveySearchParam searchParam, ISender sender) =>
            {
                var searchResults = await sender.Send(new GetSurveyBySearchQuery(searchParam));
                return Results.Ok(searchResults);
            }).WithSummary("Search All Surveys");

            //app.MapGet("surveys/{id:guid}", async (Guid id, ISender sender) =>
            //{
            //    var survey = await sender.Send(new GetBranchSurveyByIdQuery(id));
            //    return Results.Ok(survey.Value);
            //}).WithSummary("Get Branch Survey By Id");
        }
    }
}
