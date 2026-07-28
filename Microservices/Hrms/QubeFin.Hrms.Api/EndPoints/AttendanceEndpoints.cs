using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Hrms.Application.Attendances.Commands;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Application.Attendances.Queries;
using QubeFin.Persistence.Models.App;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class AttendanceEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("attendances", async (ClaimsPrincipal principal, CreateAttendanceCommand command, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var savedResult = await sender.Send(new CreateAttendanceCommand(empId, command.time, command.Lat, command.Long));
            return Results.Ok(savedResult);
        }).WithSummary("Attendance Check in and Check out Saved");

        app.MapGet("attendances", async (ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetAttendanceByEmployeeQuery(empId));
            return Results.Ok(response);
        }).WithSummary("Today's Attendance");

        app.MapPost("attendance/regularizations", async (ClaimsPrincipal principal, [FromForm] RegularizationRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var command = new CreateAttendanceRegularizationCommand(request, empId);
            var savedResult = await sender.Send(command);
            return Results.Ok(savedResult);
        }).WithSummary("Create Attendance Regularization");

        app.MapGet("attendance/regularizations/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var response = await sender.Send(new GetAttendanceRegularizationsByIdQuery(id));
            return Results.Ok(response);
        }).WithSummary("Get Attendance Regularizations by Id");

        app.MapGet("attendance/regularizations/submit/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new SubmitAttendanceRegularizationCommand(id, empId));
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
        }).WithSummary("Submit Attendance Regularization");

        app.MapGet("attendance/regularizations/decision/{id:guid}/{isApproved}", async (Guid id, bool isApproved, ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var result = await sender.Send(new ApproveRejectAttendanceRegularizationCommand(id, isApproved, userId));
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
        }).WithSummary("Approve or Reject Attendance Regularization");
    }
}
