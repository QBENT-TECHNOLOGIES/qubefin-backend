using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Grt;
using QubeFin.Global.Application.Grt.Commands;
using QubeFin.Global.Application.Grt.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    // GRT (Group Recognition Test) with the Final Group Assessment, of an Approved Group Survey.
    public class GrtEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("surveys/{id:guid}/grt", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGrtQuery(id, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Get The GRT Of A Group Survey")
            .WithTags("Grt")
            .RequireAuthorization();

            app.MapPut("surveys/{id:guid}/grt", async (Guid id, GrtRequest request, ClaimsPrincipal principal, ISender sender) =>
            {
                var userId = principal.Identity.GetUserId();
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new SaveGrtCommand(id, request, userId, employeeId));
                return result.ToHttpResult();
            })
            .WithSummary("Save Or Submit The GRT Of A Group Survey")
            .WithTags("Grt")
            .RequireAuthorization();
        }
    }
}
