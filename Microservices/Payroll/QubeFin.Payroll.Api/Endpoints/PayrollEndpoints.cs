using MediatR;
using Microsoft.AspNetCore.Authorization;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Payroll.Application.Payrolls.Commands;
using QubeFin.Payroll.Application.Payrolls.Queries;
using System.Security.Claims;
using System.Security.Principal;

namespace QubeFin.Payroll.Api.Endpoints
{
    public class PayrollEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("payrolls", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllPayrollQuery(), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get all payrolls")
              .WithDescription("Retrieves a list of all payrolls in the system.")
              .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapGet("payroll/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new GetPayrollByIdQuery(id));
                return result.ToHttpResult();
            }).WithSummary("Get a payroll by ID")
              .WithDescription("Retrieves a specific payroll by its unique identifier.")
              .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapGet("payrolls/{month:int}/{year:int}", async (int month, int year, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMonthlyPayrollQuery(month, year), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get monthly payroll")
              .WithDescription("Retrieves the monthly payroll grouped by organization unit for the given month and year.")
              .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapGet("month-wise-payroll", async (Guid? companyId, int? payrollMonth, int payrollYear, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetMonthwisePayrollSummaryQuery(companyId, payrollMonth, payrollYear), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get month wise payrolls")
              .WithDescription("Retrieves a list of month wise payrolls in the system.")
              .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapPut("lock-payrolls/{year:int}/{month:int}", async (int year, int month, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new LockPayrollCommand(month, year), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Lock monthly payrolls")
            .WithDescription("Locks all payroll data for the specified month and year, preventing further modifications.")
            .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapPost("create", async (Guid companyId, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }
                var userId = principal.Identity.GetUserId();
                var result = await sender.Send(new CreatePayrollCommand(companyId, userId), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Generate monthly payroll")
            .WithDescription("Executes the USP_CreatePayroll stored procedure to generate payrolls.")
            .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapPut("update-employee-payroll", async (UpdatePayrollComponentsCommand command, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Update employee payroll components")
            .WithDescription("Updates the earning and deduction heads for a specific employee payroll.")
            .WithTags("Payrolls");

            app.MapGet("payslips", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetPayslipsQuery(employeeId), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get last 6 months Payslips")
              .WithDescription("Retrieves a list of payslips for the last 6 months for the authenticated employee.")
              .WithTags("Payrolls")
            .RequireAuthorization();

            app.MapGet("salary-grade", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetAllSalaryGradeQuery(), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get All Salary Grade.")
              .WithDescription("Retrieves a list of Salary Grades.")
              .WithTags("Payrolls")
            .RequireAuthorization();
        }
    }
}
