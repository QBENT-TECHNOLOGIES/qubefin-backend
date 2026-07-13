using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Hrms.Application.Attendances.Commands;

namespace QubeFin.Hrms.Api.Endpoints;

public class AttendanceEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {

        app.MapPost("attendances", async (CreateAttendanceCommand command, ISender sender) =>

        {
            await sender.Send(command);

            return Results.Ok();
        })
        .WithSummary("Create Attendance");
    }
}
