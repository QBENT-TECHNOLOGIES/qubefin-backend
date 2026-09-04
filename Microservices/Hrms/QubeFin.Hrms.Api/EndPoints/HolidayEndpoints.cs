using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Holidays.Commands;
using QubeFin.Hrms.Application.Holidays.Queries;
using QubeFin.Hrms.Application.Holidays.Models;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class HolidayEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("holidays/search/{year}", async (
        ISender sender,
        CancellationToken cancellationToken,
        int year) =>
        {
            var result = await sender.Send(new SearchHolidaysQuery(year), cancellationToken);
            return result.ToHttpResult();
        })
    .WithSummary("Search holidays by year")
    .WithTags("Holidays")
    .RequireAuthorization();

        app.MapGet("holidays/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetHolidayByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get a holiday by ID")
        .WithTags("Holidays")
        .RequireAuthorization();

        app.MapPost("holidays", async (HolidayRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var userId = principal.Identity.GetUserId();
            var command = new CreateHolidayCommand(request, userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Create a holiday")
        .WithTags("Holidays")
        .RequireAuthorization();

        app.MapPut("holidays/{id:guid}", async (Guid id, UpdateHolidayCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }
            var result = await sender.Send(command with
            {
                Id = id,
                ModifiedBy = principal.Identity.GetUserId()
            }, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Update a holiday")
        .WithTags("Holidays")
        .RequireAuthorization();

        app.MapGet("holidays/my-holidays", async (ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetMyHolidaysQuery(principal.Identity.GetEmployeeId()), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get my holidays")
        .WithTags("Holidays")
        .RequireAuthorization();
    }
}
