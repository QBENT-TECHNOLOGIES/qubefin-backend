using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyCommittees.Commands;
using QubeFin.Global.Application.SurveyCommittees.Models;
using QubeFin.Global.Application.SurveyCommittees.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class SurveyCommitteeEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("survey-committees/filter", async (IMediator mediator, string? searchText, string? sortOn, string? sortDirection, int pageIndex, int pageSize) =>
            {
                var resp = await mediator.Send(new FilterCommitteeMemberQuery(searchText, sortOn, sortDirection, pageIndex, pageSize));
                return TypedResults.Ok(resp);
            }).WithSummary("Filter Committee Members");

            app.MapGet("survey-committees/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetByIdQuery(id));
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
            }).WithSummary("Get Committee Member By Id");

            app.MapPost("survey-committees", async (MemberAddRequest request, IMediator mediator, ClaimsPrincipal principal) =>
            {
                var result = await mediator.Send(new AddMemberCommand(request, principal.Identity.GetUserId()));
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
            }).WithSummary("Add Member To Survey Committee");

            app.MapPut("survey-committees", async (MemberUpdateRequest request, IMediator mediator, ClaimsPrincipal principal) =>
            {
                var result = await mediator.Send(new UpdateMemberCommand(request, principal.Identity.GetUserId()));
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
            }).WithSummary("Update Member To Survey Committee");
        }
    }
}
