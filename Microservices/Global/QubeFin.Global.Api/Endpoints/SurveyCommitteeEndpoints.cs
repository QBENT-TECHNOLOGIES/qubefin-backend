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
            app.MapGet("survey-committees/filter", async (ISender sender, string? searchText, string? sortOn, string? sortDirection, int pageIndex, int pageSize) =>
            {
                var result = await sender.Send(new FilterCommitteeMemberQuery(searchText, sortOn, sortDirection, pageIndex, pageSize));
                return Results.Ok(result);
            })
            .WithSummary("Filter Committee Members")
            .WithTags("SurveyCommittees")
            .RequireAuthorization();

            app.MapGet("survey-committees/{id:guid}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetByIdQuery(id));
                return result.ToHttpResult();
            })
            .WithSummary("Get Committee Member By Id")
            .WithTags("SurveyCommittees")
            .RequireAuthorization();

            app.MapPost("survey-committees", async (MemberAddRequest request, ISender sender, ClaimsPrincipal principal) =>
            {
                var result = await sender.Send(new AddMemberCommand(request, principal.Identity.GetUserId()));
                return result.ToHttpResult();
            })
            .WithSummary("Add Member To Survey Committee")
            .WithTags("SurveyCommittees")
            .RequireAuthorization();

            app.MapPut("survey-committees", async (MemberUpdateRequest request, ISender sender, ClaimsPrincipal principal) =>
            {
                var result = await sender.Send(new UpdateMemberCommand(request, principal.Identity.GetUserId()));
                return result.ToHttpResult();
            })
            .WithSummary("Update Member To Survey Committee")
            .WithTags("SurveyCommittees")
            .RequireAuthorization();
        }
    }
}
