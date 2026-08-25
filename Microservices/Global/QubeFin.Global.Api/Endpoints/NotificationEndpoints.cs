using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Notifications.Commands;
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
                var result = await sender.Send(new GetAllQuery(employeeId), cancellationToken);
                return result.ToHttpResult();
            }).WithName("GetAll")
            .WithSummary("Get All Notifications")
            .WithDescription("Returns all notifications for the authenticated user.")
            .WithTags("Notifications")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

            app.MapGet("notifications/count", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetNotificationCountQuery(employeeId), cancellationToken);
                return result.ToHttpResult();
            }).WithName("GetNotificationCount")
            .WithSummary("Get Notification Count")
            .WithDescription("Returns the count of notifications for the authenticated user.")
            .WithTags("Notifications")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization();

            app.MapGet("notifications/{notificationId:guid}/read", async (ISender sender, ClaimsPrincipal principal, Guid notificationId, CancellationToken cancellationToken) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new ReadNotificationCommand(notificationId), cancellationToken);
                return result.ToHttpResult();
            }).WithName("MarkAsRead")
            .WithSummary("Mark Notification as Read")
            .WithDescription("Marks a specific notification as read for the authenticated user.")
            .WithTags("Notifications")
            .RequireAuthorization();

            app.MapGet("notifications/read-all", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new ReadAllNotificationCommand(employeeId), cancellationToken);
                return result.ToHttpResult();
            }).WithName("MarkAllAsRead")
            .WithSummary("Mark All Notifications as Read")
            .WithDescription("Marks all notifications as read for the authenticated user.")
            .WithTags("Notifications")
            .RequireAuthorization();
        }
    }
}
