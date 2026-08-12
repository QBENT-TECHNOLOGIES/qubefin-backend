using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Api.Requests;
using QubeFin.Hrms.Application.LeaveApproval.Models;
using QubeFin.Hrms.Application.LeavePrayers.Commands;
using QubeFin.Hrms.Application.LeavePrayers.Models;
using QubeFin.Hrms.Application.LeavePrayers.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.EndPoints
{
    public class LeavePrayerEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("leave/prayers", async (ClaimsPrincipal principal, [FromForm] LeavePrayerRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var empId = principal.Identity.GetEmployeeId();
                var userId = principal.Identity.GetUserId();
                var command = new ApplyLeavePrayerCommand(request, empId, userId);
                var result = await sender.Send(command);
                if (result.IsFailed)
                {
                    if (result.Errors[0] is QubeFin.Core.Results.RecordNotFoundError)
                    {
                        return Results.NotFound(result.Errors[0]);
                    }
                    if (result.Errors[0] is QubeFin.Core.Results.ValidationError)
                    {
                        return Results.BadRequest(result.Errors[0]);
                    }
                }
                return Results.Ok(result.Value);
            }).DisableAntiforgery().WithSummary("Apply Leave prayer").RequireAuthorization().WithTags("Leave Prayers");

            app.MapGet("leave/prayers/by-year/{year}", async (ClaimsPrincipal principal, ISender sender, [FromRoute] int year, CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }

                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetPrayerByEmployeeIdQuery(year, employeeId));
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithSummary("Get year wise Leave Payers for specific employee")
            .WithTags("Leave Prayers");

            app.MapGet("leave/prayers/{id}/{employeeId}", async (ClaimsPrincipal principal, IMediator mediator, Guid id, Guid employeeId) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }

                var result = await mediator.Send(new GetPrayerByIdQuery(id, employeeId));
                return result.ToHttpResult();
            })
            .RequireAuthorization()
            .WithSummary("Get Leave Prayer Detail")
            .WithTags("Leave Prayers");

            #region --- LEAVE PRAYER APPROVAL ---

            app.MapPost("leave/prayers/approval/search", async (ClaimsPrincipal principal, ISender sender, LeaveApprovalSearchRequest request) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }

                var empId = principal.Identity.GetEmployeeId();
                var response = await sender.Send(new GetLeavePrayerApprovalListByEmployeeIdQuery(empId, request));
                return Results.Ok(response);
            }).WithSummary("Get Leave Approval List for logged-in employee with optional filters")
            .WithTags("Leave Prayer Approval");

            app.MapPost("leave/prayers/action", async (ClaimsPrincipal principal, ISender sender, LeavePrayerActionRequest request) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }

                var userId = principal.Identity.GetUserId();
                var response = await sender.Send(new LeavePrayerActionCommand(request.LeavePrayerId, request.IsApproved, request.IsRejected, userId));
                return Results.Ok(response);
            })
            .WithSummary("Leave Request Action")
            .WithTags("Leave Prayer Approval");
            #endregion
        }
    }
}
