using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.Attendances.Queries;
using QubeFin.Hrms.Application.Leaves.Commands;
using QubeFin.Hrms.Application.Leaves.Models;
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
        .WithSummary("Get Leave Requests for Loggedin Employee")
        .WithTags("Leave Requests");

        app.MapGet("leaves/requests/{id}/{employeeId}", async (ClaimsPrincipal principal, IMediator mediator, Guid id, Guid employeeId) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var result = await mediator.Send(new GetRequestByIdQuery(id, employeeId));
            return result.ToHttpResult();
        })
        .RequireAuthorization()
        .WithSummary("Get Leave Request Detail")
        .WithTags("Leave Requests");
        #endregion
        #region --- REQUESTS ---

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
        #region --- SUBMIT ---

        app.MapGet("leaves/submit/{id}", async (ClaimsPrincipal principal, ISender sender, Guid id) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var response = await sender.Send(new SubmitRequestCommand(id, userId));
            return Results.Ok(response);
        })
        .WithSummary("Leave Request Submit")
        .WithTags("Leave Requests");
        #endregion
        #region --- CANCEL ---

        app.MapPost("leaves/cancel/{id}", async (ClaimsPrincipal principal, ISender sender, CancelLeaveRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var response = await sender.Send(new CancelRequestCommand(request.Id, request.Reason, userId));
            return Results.Ok(response);
        })
        .WithSummary("Leave Request Cancel")
        .WithTags("Leave Requests");
        #endregion
        #region --- ACTION ---

        app.MapPost("leaves/action", async (ClaimsPrincipal principal, ISender sender, LeaveRequestActionRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var response = await sender.Send(new LeaveRequestActionCommand(request.LeaveRequestId, request.IsApproved, request.IsRejected, userId, request.RejectedReason));
            return Results.Ok(response);
        })
        .WithSummary("Leave Request Action")
        .WithTags("Leave Requests");
        #endregion
    }
}