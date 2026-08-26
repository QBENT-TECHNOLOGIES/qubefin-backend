using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.LeaveTypes.Queries;
using System.Security.Claims;

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
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("leave-types/balances/{employeeId:guid}", async (ClaimsPrincipal principal, ISender sender,Guid employeeId, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetLeaveTypesByEmployeeIdQuery(employeeId));
            return result.ToHttpResult();
        })
        .WithSummary("Get all Leave balances by leave Types for specific employee")
        .WithDescription("Retrieves a list of all leave balances of an employee for all leave types in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("leave-types/balances", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetLeaveTypesByEmployeeIdQuery(employeeId));
            return result.ToHttpResult();
        })
        .WithSummary("Get all Leave balances by leave Types for specific employee")
        .WithDescription("Retrieves a list of all leave balances of an employee for all leave types in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("leave-types/prayer-balances", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetLeavePrayerTypeByEmployeeIdQuery(employeeId));
            return result.ToHttpResult();
        })
        .WithSummary("Get Leave Payer balances by leave Types for specific employee")
        .WithDescription("Retrieves a list of leave prayer balances of an employee for all leave types in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("posts", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAllPostQuery());
            return result.ToHttpResult();
        })
        .WithSummary("Get all Posts")
        .WithDescription("Retrieves a list of all posts in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("leave-types/type-wise-balances/by-employee", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetLeaveTypeWiseBalanceQuery(employeeId));
            return result.ToHttpResult();
        })
        .WithSummary("Get all leave Type wise balance for specific employee")
        .WithDescription("Retrieves a list of all leave balances of an employee for all leave types in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();

        app.MapGet("leave-types/type-wise-transaction/by-employee/{id:guid}", async (ClaimsPrincipal principal, ISender sender, Guid id,CancellationToken cancellationToken) =>
        {
            var employeeId = principal.Identity.GetEmployeeId();
            var result = await sender.Send(new GetLeaveWiseTransactionQuery(employeeId, id));
            return result.ToHttpResult();
        })
        .WithSummary("Get leave Type wise transaction for specific employee")
        .WithDescription("Retrieves a list of leave balances of an employee for a specific leave type in the system.")
        .WithTags("Leave Types")
        .RequireAuthorization();
    }
}
