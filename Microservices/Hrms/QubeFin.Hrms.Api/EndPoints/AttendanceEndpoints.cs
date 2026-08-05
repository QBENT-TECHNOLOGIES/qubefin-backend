using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Hrms.Application.Attendances.Commands;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Application.Attendances.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class AttendanceEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("attendances/punch", async (ClaimsPrincipal principal, CreateAttendanceCommand command, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new CreateAttendanceCommand(empId, command.time, command.Lat, command.Long));
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
        }).WithSummary("Attendance Check in and Check out Saved").WithTags("Attendance");

        app.MapGet("attendances", async (ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetAttendanceByEmployeeQuery(empId));
            return Results.Ok(response.Value);
        }).WithSummary("Today's Attendance").WithTags("Attendance");

        app.MapPost("attendances/history", async (ClaimsPrincipal principal, ISender sender, AttendanceSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetAttendanceHistoryByQuery(empId, request));
            return Results.Ok(response);
        }).WithSummary("Get attendance history for logged-in employee with optional filters").WithTags("Attendance");

        app.MapPost("attendances/regularizations", async (ClaimsPrincipal principal, [FromForm] RegularizationRequest request, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var userId = principal.Identity.GetUserId();
            var command = new CreateAttendanceRegularizationCommand(request, empId, userId);
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
        }).DisableAntiforgery().WithSummary("Create Attendance Regularization").WithTags("Regularization");

        app.MapPost("attendances/regularizations/search", async (ClaimsPrincipal principal, ISender sender, AttendanceSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetAttendanceRegularizationBySearch(empId, request));
            return Results.Ok(response);
        }).WithSummary("Search attendance regularization for logged-in employee with optional filters").WithTags("Regularization"); ;

        app.MapGet("attendances/regularizations/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetAttendanceRegularizationsByIdQuery(id, empId));
            return Results.Ok(response.Value);
        }).WithSummary("Get Attendance Regularizations by Id").WithTags("Regularization");

        app.MapPost("attendances/regularizations/search-approval", async (ClaimsPrincipal principal, ISender sender, AttendanceApprovalSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var response = await sender.Send(new GetApprovalRegularizationBySearch(empId, request));
            return Results.Ok(response);
        }).WithSummary("Search approvals regularization for logged-in employee with optional filters").WithTags("Regularization");

        app.MapPost("attendances/regularizations/submit", async (ClaimsPrincipal principal, ISender sender, RegularizationSubmit request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var CurrentUserId = principal.Identity.GetUserId();
            var response = await sender.Send(new SubmitAttendanceRegularizationCommand(request, empId, CurrentUserId));
            return Results.Ok(response);
        }).WithSummary("Decision regularization (Approved/Reject/Recommend)").WithTags("Regularization");
    }
}
