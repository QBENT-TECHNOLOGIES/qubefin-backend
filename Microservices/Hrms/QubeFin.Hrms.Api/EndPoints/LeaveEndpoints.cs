using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.LeaveApproval.Models;
using QubeFin.Hrms.Application.LeaveApproval.Queries;
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

            Guid? id = null;

            if (!string.IsNullOrWhiteSpace(form["id"]))
            {
                if (!Guid.TryParse(form["id"], out var parsedId))
                {
                    return Results.BadRequest("Invalid id.");
                }

                id = parsedId;
            }

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

            var result = await sender.Send(new SaveRequestCommand(id, employeeId, leaveTypeId, fromDate, toDate, address, reason, enclosedFileName, enclosedFile));
            return result.ToHttpResult();
        })
        .WithSummary("Create And save Leave Request")
        .WithTags("Leave Requests")
        .RequireAuthorization();
        #endregion
        #region --- SUBMIT ---

        app.MapGet("leaves/requests/submit/{id}", async (ClaimsPrincipal principal, ISender sender, Guid id) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new SubmitRequestCommand(id, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Leave Request Submit")
        .WithTags("Leave Requests")
        .RequireAuthorization();
        #endregion
        #region --- CANCEL ---

        app.MapPost("leaves/requests/cancel", async (ClaimsPrincipal principal, ISender sender, CancelLeaveRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new CancelRequestCommand(request.Id, request.Reason, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Leave Request Cancel")
        .WithTags("Leave Requests")
        .RequireAuthorization();
        #endregion
        #region --- LEAVE APPROVAL ---

        app.MapPost("leaves/approval/search", async (ClaimsPrincipal principal, ISender sender, LeaveApprovalSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetLeaveApprovalsByEmployeeIdQuery(empId, request));
            return Results.Ok(result);
        })
        .WithSummary("Get Leave Approval List for logged-in employee with optional filters")
        .WithTags("Leave Approval")
        .RequireAuthorization();

        app.MapPost("leaves/action", async (ClaimsPrincipal principal, ISender sender, LeaveRequestActionRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new LeaveRequestActionCommand(request.LeaveRequestId, request.IsApproved, request.IsRejected, userId, request.RejectedReason));
            return result.ToHttpResult();
        })
        .WithSummary("Leave Request Action")
        .WithTags("Leave Approval")
        .RequireAuthorization();
        #endregion


        #region --- LEAVE FITNESS ---

        app.MapGet("leaves/fitness-approval", async (ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var result = await sender.Send(new GetAllPendingFitnessApprovalQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get All Pending Fitness Approval list for employees")
        .WithTags("Fitness Approval")
        .RequireAuthorization();

        app.MapPost("leaves/fitnes-upload", async (ClaimsPrincipal principal, HttpRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var employeeId = principal.Identity.GetEmployeeId();

            if (!request.HasFormContentType)
                return Results.BadRequest("Invalid content type");

            var form = await request.ReadFormAsync();
            var fitnessReportAttachment = form.Files["fitnessReportAttachment"];

            var result = await sender.Send(new UploadFitnessReportCommand(employeeId, fitnessReportAttachment));
            return result.ToHttpResult();
        })
        .WithSummary("Upload Fitness Report For Specific Leave.")
        .WithTags("Fitness Approval")
        .RequireAuthorization();

        app.MapGet("leaves/fitnes-upload/action/{leaveRequestId:guid}", async (ClaimsPrincipal principal, ISender sender, Guid leaveRequestId) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new FitnessReportActionCommand(leaveRequestId, userId));
            return result.ToHttpResult();
        })
        .WithSummary("Action Fitness Report For Specific Leave.")
        .WithTags("Fitness Approval")
        .RequireAuthorization();
        #endregion
    }
}