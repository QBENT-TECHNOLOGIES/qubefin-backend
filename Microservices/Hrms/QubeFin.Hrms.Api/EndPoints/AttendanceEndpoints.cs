using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Hrms.Application.Attendances.Commands;
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
            await sender.Send(new CreateAttendanceCommand(empId, command.time, command.Lat, command.Long));

            return Results.Ok();
        })
        .WithSummary("Create Attendance");
    }
}
