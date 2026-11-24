using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Cgt;
using QubeFin.Global.Application.Cgt.Commands;
using QubeFin.Global.Application.Cgt.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    // CGT (Compulsory Group Training) of an Approved Group Survey.
    public class CgtEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/cgt", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetCgtQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get The CGT Of A Group Survey")
            .WithTags("Cgt")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/cgt", async (Guid id, CgtRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveCgtCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Or Complete The CGT Of A Group Survey")
            .WithTags("Cgt")
            .RequireAuthorization();
        }
    }
}
