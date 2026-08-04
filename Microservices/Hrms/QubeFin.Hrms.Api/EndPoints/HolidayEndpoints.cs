using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Holidays.Commands;
using QubeFin.Hrms.Application.Holidays.Queries;
using System.Security.Claims;

namespace QubeFin.Hrms.Api.Endpoints;

public class HolidayEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("holidays/search", async (
            ISender sender,
            CancellationToken cancellationToken,
            Guid? orgUnitId,
            DateOnly? fromDate,
            DateOnly? toDate,
            string? searchText,
            int pageIndex = 1,
            int pageSize = 10) =>
        {
            var result = await sender.Send(
                new SearchHolidaysQuery(orgUnitId, fromDate, toDate, searchText, pageIndex, pageSize),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithSummary("Search holidays")
        .WithDescription("Searches holidays by organization unit, date range, and description.")
        .WithTags("Holidays");

        app.MapGet("holidays/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetHolidayByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Get a holiday by ID")
        .WithTags("Holidays");

        app.MapPost("holidays", async (CreateHolidayCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
        {
            if (principal.Identity is null || !principal.Identity.IsAuthenticated)
            {
                return Results.Forbid();
            }

            var result = await sender.Send(command with { CreatedBy = principal.Identity.GetUserId() }, cancellationToken);
            return result.ToHttpResult();
        })
        .WithSummary("Create a holiday")
        .WithTags("Holidays");

        app.MapPatch("holidays/{id:guid}", async (Guid id, UpdateHolidayCommand command, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken) =>
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
        .WithTags("Holidays");
    }
}
