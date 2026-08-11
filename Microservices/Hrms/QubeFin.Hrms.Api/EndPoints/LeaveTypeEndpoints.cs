using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.LeaveTypes.Queries;

namespace QubeFin.Hrms.Api.EndPoints;

public class LeaveTypeEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("leave-types", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetLeaveTypesQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get all Leave Types")
        .WithDescription("Retrieves a list of all leave types in the system.")
        .WithTags("Leave Types");

        app.MapGet("leave-types/balances/{id}", async (ISender sender, [FromRoute] Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetLeaveTypesByEmployeeIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get all Leave balances by leave Types for specific employee")
        .WithDescription("Retrieves a list of all leave balances of an employee for all leave types in the system.")
        .WithTags("Leave Types");

        app.MapGet("leave-types/prayer-balances/{id}", async (ISender sender, [FromRoute] Guid id, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetLeavePrayerTypeByEmployeeIdQuery(id));
            return result.ToHttpResult();
        })
        .WithSummary("Get Leave Payer balances by leave Types for specific employee")
        .WithDescription("Retrieves a list of leave prayer balances of an employee for all leave types in the system.")
        .WithTags("Leave Types");
    }
}
