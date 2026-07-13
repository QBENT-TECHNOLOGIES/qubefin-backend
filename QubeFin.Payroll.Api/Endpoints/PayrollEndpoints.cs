using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Payroll.Application.Payrolls.Queries;
using System.Reflection;

namespace QubeFin.Payroll.Api.Endpoints
{
    public class PayrollEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("payrolls", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllPayrollQuery(), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get all payrolls")
              .WithDescription("Retrieves a list of all payrolls in the system.")
              .WithTags("Payrolls");
            app.MapGet("payroll/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetPayrollByIdQuery(id));
                return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Errors);
            }).WithSummary("Get a payroll by ID")
              .WithDescription("Retrieves a specific payroll by its unique identifier.")
              .WithTags("Payrolls");
            app.MapGet("payrolls/{month:int}/{year:int}", async (int month, int year, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMonthlyPayrollQuery(month, year), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get monthly payroll")
              .WithDescription("Retrieves the monthly payroll grouped by organization unit for the given month and year.")
              .WithTags("Payrolls");
            app.MapGet("month-wise-payroll", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMonthwisePayrollSummaryQuery(), cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Get month wise payrolls")
              .WithDescription("Retrieves a list of month wise payrolls in the system.")
              .WithTags("Payrolls");
        }
    }
}
