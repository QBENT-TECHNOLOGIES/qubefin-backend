using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.GroupCreation;
using QubeFin.Global.Application.GroupCreation.Commands;
using QubeFin.Global.Application.GroupCreation.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    // Group Creation: after a GRT Pass, a Credit Officer of the survey creates the group.
    public class GroupCreationEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/group-create-info", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupCreateInfoQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get What The Group Creation Window Needs")
            .WithTags("GroupCreation")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/group-create", async (Guid id, CreateGroupFromSurveyRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new CreateGroupFromSurveyCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Create The Group Of A Group Survey After A GRT Pass")
            .WithTags("GroupCreation")
            .RequireAuthorization();
        }
    }
}
