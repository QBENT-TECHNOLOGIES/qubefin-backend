using MediatR;
using Microsoft.AspNetCore.Authorization;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Payroll.Application.Payrolls.Commands;
using QubeFin.Payroll.Application.Payrolls.Queries;
using QubeFin.Payroll.Application.Payrolls.Report;
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


            #region SSRS REPORTS

            app.MapGet("/reports/payslip/{payslipId:guid}", [Authorize] async (Guid payslipId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Employee_Payslip",         //Report name
                "PDF",                          //Report Format
                new Dictionary<string, string>  //Report Parameter
                {
                    ["PayslipId"] = payslipId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate payslip report.");
            #endregion

            #region NPOI REPORTS
            app.MapGet("/generate-pf-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (int month, int year, Guid companyId, ISender sender) =>
            {
                var command = new GenerateNPOIReportsCommand("Payroll.USP_GetPFReport",
                    new Dictionary<string, object?>
                    {
                        ["@Month"] = month,
                        ["@Year"] = year
                    },
                    "PF Report",
                    $"Month: {month}, Year: {year}",
                    true,
                    companyId
                );

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, $"PF_Report_{month}_{year}.xlsx");
            }).WithSummary("Generate PF Report.");

            app.MapGet("/generate-esi-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (int month, int year, Guid companyId, ISender sender) =>
            {
                var command = new GenerateNPOIReportsCommand("Payroll.USP_GetESIReport",
                    new Dictionary<string, object?>
                    {
                        ["@Month"] = month,
                        ["@Year"] = year
                    },
                    "ESI Report",
                    $"Month: {month}, Year: {year}",
                    true,
                    companyId
                );

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, $"ESI_Report_{month}_{year}.xlsx");
            }).WithSummary("Generate ESI Report.");

            app.MapGet("/generate-ptax-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (int month, int year, Guid companyId, ISender sender) =>
            {
                var command = new GenerateNPOIReportsCommand("Payroll.USP_GetProfTaxReport",
                    new Dictionary<string, object?>
                    {
                        ["@Month"] = month,
                        ["@Year"] = year
                    },
                    "Professional Tax Report",
                    $"Month: {month}, Year: {year}",
                    true,
                    companyId
                );

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, $"Professional_Tax_Report_{month}_{year}.xlsx");
            }).WithSummary("Generate Professional Tax Report.");

            app.MapGet("/generate-salary-disbursement-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (ClaimsPrincipal principal,int month, int year, Guid companyId,ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var command = new GenerateSalaryDisbursementSheetCommand("Payroll.USP_SalaryDisbursementSheet",
                    new Dictionary<string, object?>
                    {
                        ["@p_month"] = month,
                        ["@p_year"] = year,
                        ["@p_companyId"] = companyId
                    },
                    month,
                    year,
                    companyId,
                    employeeId
                );

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, $"Salary_Disbursement_Sheet_{month}_{year}.xlsx");
            }).WithSummary("Generate Salary Disbursement Sheet Report.");

            app.MapGet("/generate-salary-register-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (ClaimsPrincipal principal, int month, int year, Guid companyId, ISender sender) =>
            {
                var employeeId = principal.Identity.GetEmployeeId();
                var command = new GenerateEmployeeSalaryRegisterCommand("Hrms.USP_EmployeeSalaryRegister",
                    new Dictionary<string, object?>
                    {
                        ["@p_Month"] = month,
                        ["@p_Year"] = year,
                        ["@p_companyId"] = companyId
                    },
                    companyId,
                    month,
                    year
                );

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, $"Salary_Register_Report_{month}_{year}.xlsx");
            }).WithSummary("Generate Salary Register Report.");
            #endregion
        }
    }
}
