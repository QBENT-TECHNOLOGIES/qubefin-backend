using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
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
            return result.ToHttpResult();
        })
        .WithSummary("Attendance Check in and Check out Saved")
        .WithTags("Attendance")
        .RequireAuthorization();

        app.MapGet("attendances", async (ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetAttendanceByEmployeeQuery(empId));
            return result.ToHttpResult();
        }).WithSummary("Today's Attendance").WithTags("Attendance").RequireAuthorization();

        app.MapPost("attendances/history", async (ClaimsPrincipal principal, ISender sender, AttendanceSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetAttendanceHistoryByEmployee(empId, request));
            return Results.Ok(result);
        }).WithSummary("Get attendance history for logged-in employee with optional filters").WithTags("Attendance").RequireAuthorization();

        app.MapPost("attendances/history-all", async (ClaimsPrincipal principal, ISender sender, AttendanceSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetAttendanceHistoryByEmployee(empId, request));
            return Results.Ok(result);
        }).WithSummary("Get all employees attendance history with optional filters").WithTags("Attendance").RequireAuthorization();

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
            return result.ToHttpResult();
        }).DisableAntiforgery().WithSummary("Create Attendance Regularization").WithTags("Regularization").RequireAuthorization();

        app.MapPost("attendances/regularizations/search", async (ClaimsPrincipal principal, ISender sender, AttendanceSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetAttendanceRegularizationBySearch(empId, request));
            return Results.Ok(result);
        }).WithSummary("Search attendance regularization for logged-in employee with optional filters").WithTags("Regularization"); ;

        app.MapGet("attendances/regularizations/{id:guid}", async (Guid id, ClaimsPrincipal principal, ISender sender) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }
            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetAttendanceRegularizationsByIdQuery(id, empId));
            return result.ToHttpResult();
        })
        .WithSummary("Get Attendance Regularizations by Id")
        .WithTags("Regularization")
        .RequireAuthorization();

        app.MapPost("attendances/regularizations/search-approval", async (ClaimsPrincipal principal, ISender sender, AttendanceApprovalSearchRequest request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetApprovalRegularizationBySearch(empId, request));
            return Results.Ok(result);
        })
        .WithSummary("Search approvals regularization for logged-in employee with optional filters")
        .WithTags("Regularization")
        .RequireAuthorization();

        app.MapPost("attendances/regularizations/submit", async (ClaimsPrincipal principal, ISender sender, RegularizationSubmit request) =>
        {
            if (principal.Identity is null)
            {
                return Results.Forbid();
            }

            var empId = principal.Identity.GetEmployeeId();
            var CurrentUserId = principal.Identity.GetUserId();
            var result = await sender.Send(new SubmitAttendanceRegularizationCommand(request, empId, CurrentUserId));
            return result.ToHttpResult();
        })
        .WithSummary("Decision regularization (Approved/Reject/Recommend)")
        .WithTags("Regularization")
        .RequireAuthorization();
    }
}
