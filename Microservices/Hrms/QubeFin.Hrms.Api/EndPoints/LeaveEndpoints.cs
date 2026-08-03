using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Leaves.Commands;
using QubeFin.Hrms.Application.Leaves.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.EndPoints;

public class LeaveEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        #region --- REQUESTS ---
        app.MapGet("leaves/requests/by-year/{year}", async (ClaimsPrincipal principal, IMediator mediator, int year) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var employeeId = principal.Identity.GetEmployeeId();
            var result = await mediator.Send(new GetRequestsByEmployeeIdQuery(year, employeeId));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithSummary("Get Leave Requests for Loggedin Employee");

        app.MapPost("leaves/requests", async (ClaimsPrincipal principal, HttpRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var employeeId = principal.Identity.GetEmployeeId();

            if (!request.HasFormContentType)
                return Results.BadRequest("Invalid content type");

            var form = await request.ReadFormAsync();

            if (!Guid.TryParse(form["leaveTypeId"], out var leaveTypeId))
                return Results.BadRequest("Invalid leaveTypeId");

            if (!DateOnly.TryParse(form["fromDate"], out var fromDate))
                return Results.BadRequest("Invalid fromDate");

            if (!DateOnly.TryParse(form["toDate"], out var toDate))
                return Results.BadRequest("Invalid toDate");

            var address = form["address"].ToString();
            var reason = form["reason"].ToString();
            var enclosedFileName = form["enclosedFileName"].ToString();
            var enclosedFile = form.Files["enclosedFile"];

            var result = await sender.Send(new CreateRequestCommand(employeeId, leaveTypeId, fromDate, toDate, address, reason, enclosedFileName, enclosedFile));
            return result.ToHttpResult();
        })
        .WithSummary("Creates New Leave Request")
        .WithTags("Leave Requests");
        #endregion
    }
}
