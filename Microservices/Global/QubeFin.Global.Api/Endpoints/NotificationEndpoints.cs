using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Global.Application.Notifications.Queries;
using System.Security.Claims;

namespace QubeFin.Global.Api.Endpoints
{
    public class NotificationEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("notifications", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetAllUnreadQuery(employeeId), cancellationToken);
                if (result.IsFailed)
                {
                    return Results.Problem("Failed to retrieve Organization Unit tree");
                }
                return Results.Ok(result.Value);
            }).WithName("GetAllUnread")
            .WithSummary("Get All Unread Notifications")
            .WithDescription("Returns the all unread notifications for the authenticated user.")
            .WithTags("Notifications")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
