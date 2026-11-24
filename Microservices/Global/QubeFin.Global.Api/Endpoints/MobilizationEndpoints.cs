using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Mobilization;
using QubeFin.Global.Application.Mobilization.Commands;
using QubeFin.Global.Application.Mobilization.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    // Community Mobilization: the prospective borrowers of an Approved Group Survey.
    public class MobilizationEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/mobilization", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetMobilizationQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get Community Mobilization Borrowers Of A Group Survey")
            .WithTags("Mobilization")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/borrowers", async (Guid id, BorrowerRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveBorrowerCommand(id, null, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Add A Prospective Borrower")
            .WithTags("Mobilization")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/borrowers/{borrowerId:guid}", async (Guid id, Guid borrowerId, BorrowerRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveBorrowerCommand(id, borrowerId, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Update A Prospective Borrower")
            .WithTags("Mobilization")
            .RequireAuthorization();

            app.MapPost("surveys/{id:guid}/borrowers/{borrowerId:guid}/decision", async (Guid id, Guid borrowerId, BorrowerDecisionRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new ChangeBorrowerDecisionCommand(id, borrowerId, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Head Of Branch Changes Add Or Block Of A Flagged Borrower")
            .WithTags("Mobilization")
            .RequireAuthorization();
        }
    }
}
