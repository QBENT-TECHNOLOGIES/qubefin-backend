using MediatR;
using Microsoft.AspNetCore.Mvc;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.AttendanceMoralization.Commands;
using QubeFin.Hrms.Application.AttendanceMoralization.Models;
using QubeFin.Hrms.Application.AttendanceMoralization.Queries;

namespace QubeFin.Payroll.Api.Endpoints
{
    public class AttendanceMoralizationEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("attendance-moralization", async (ISender sender, [FromBody] MoralizationSearch searchParam, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllByMonthAndYearQuery(searchParam), cancellationToken);
                return TypedResults.Ok(result);
            }).WithSummary("Get monthly attendance moralization")
              .WithDescription("Retrieves the monthly attendance moralization for the given month and year.")
              .WithTags("Attendance Moralization")
            .RequireAuthorization();

            app.MapGet("attendance-moralization/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetMoralizationByIdQuery(id));
                return result.ToHttpResult();
            }).WithSummary("Get a moralization by ID")
              .WithDescription("Retrieves a specific moralization by its unique identifier.")
              .WithTags("Attendance Moralization")
            .RequireAuthorization();

            app.MapGet("attendance-moralization/generate/{month}/{year}", async (int month, int year, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GenerateMoralizationCommand(month, year), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Generate a moralization")
              .WithDescription("Generates a new moralization for the specified month and year.")
              .WithTags("Attendance Moralization")
            .RequireAuthorization();

            app.MapPost("attendance-moralization/update/{id}", async (Guid id, ISender sender, [FromBody] List<EmployeeLosDetails> updateParam, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new UpdateMoralizationCommand(id, updateParam), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Update a moralization")
              .WithDescription("Updates an existing moralization with the provided details.")
              .WithTags("Attendance Moralization")
            .RequireAuthorization();

            app.MapGet("attendance-moralization/lock/{month}/{year}", async (int month, int year, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new LockMoralizationCommand(month, year), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Lock a moralization")
              .WithDescription("Locks the moralization data for the specified month and year.")
              .WithTags("Attendance Moralization")
            .RequireAuthorization();
        }
    }
}
