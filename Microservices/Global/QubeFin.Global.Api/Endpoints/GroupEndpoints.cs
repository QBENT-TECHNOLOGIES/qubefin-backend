using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Groups.Commands;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Global.Application.Groups.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public record GroupSearchRequest(string? SearchText, string? Status, string? SortOn, string? SortDirection, int PageIndex = 0, int PageSize = 10);

    // Group Master (Member menu): list, view, add, edit, approve / reject.
    public class GroupEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("groups/search", async (GroupSearchRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SearchGroupsQuery(request.SearchText, request.Status, request.SortOn, request.SortDirection,
                    request.PageIndex, request.PageSize, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Search Groups")
            .WithTags("Groups")
            .RequireAuthorization();

            app.MapGet("groups/add-info", async (ClaimsPrincipal principal, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupAddInfoQuery(employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Can The User Add A Group, And The Choices For The Add Group Form")
            .WithTags("Groups")
            .RequireAuthorization();

            app.MapGet("groups/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupByIdQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Group By Id")
            .WithTags("Groups")
            .RequireAuthorization();

            app.MapPost("groups", async (GroupRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new CreateGroupCommand(request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Add Group")
            .WithTags("Groups")
            .RequireAuthorization();

            app.MapPut("groups/{id:guid}", async (Guid id, GroupRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new UpdateGroupCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Update Group")
            .WithTags("Groups")
            .RequireAuthorization();

            app.MapPost("groups/{id:guid}/decision", async (Guid id, GroupDecisionRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new DecideGroupCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Approve Or Reject A Group")
            .WithTags("Groups")
            .RequireAuthorization();
        }
    }
}
