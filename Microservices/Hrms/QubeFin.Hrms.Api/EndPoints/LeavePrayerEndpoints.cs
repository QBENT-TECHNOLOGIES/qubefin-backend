using Amazon.Auth.AccessControlPolicy;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Attendances.Models;
using QubeFin.Hrms.Application.LeavePrayers.Commands;
using QubeFin.Hrms.Application.LeavePrayers.Models;
using QubeFin.Hrms.Application.LeavePrayers.Queries;
using QubeFin.Hrms.Application.LeaveTypes.Queries;
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
            }).DisableAntiforgery().WithSummary("Apply Leave prayer").WithTags("Leave Prayer");

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
            .WithSummary("Get year wise Leave Payers for specific employee")
            .WithTags("Leave Prayer");
        }
    }
}
