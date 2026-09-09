using MediatR;
using Microsoft.AspNetCore.Authorization;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Report.Application.Reports;
using QubeFin.Report.Application.Reports.Generate.CustomNPOIReports;
using QubeFin.Report.Application.Reports.Generate.LinqNPOIReport;
using QubeFin.Report.Application.Reports.Generate.NPOIReports;
using QubeFin.Report.Application.Reports.Generate.SSRSReports;
using QubeFin.Report.Application.Reports.Models;
using QubeFin.Report.Application.Reports.Queries;
using System.Security.Claims;

namespace QubeFin.Report.Api.Endpoints
{
    public class ReportEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            #region SSRS REPORTS

            app.MapGet("/payslip/{payslipId:guid}", [Authorize] async (Guid payslipId, ISender sender) =>
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

            app.MapGet("/generate-salary-disbursement-report/{month:int}/{year:int}/{companyId:Guid}", [Authorize] async (ClaimsPrincipal principal, int month, int year, Guid companyId, ISender sender) =>
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

            app.MapPost("/generate-attendance-history-report", [Authorize] async (ClaimsPrincipal principal, AttendanceSearchRequest request, ISender sender) =>
            {
                if (principal.Identity is null)
                {
                    return Results.Forbid();
                }
                var empId = principal.Identity.GetEmployeeId();

                var attendanceResult = await sender.Send(new GetAttendanceHistoryByQuery(request, empId));

                if (attendanceResult.IsFailed)
                    return attendanceResult.ToHttpResult();

                var command = new GenerateLinqNPOIReportCommand<AttendanceSearchResult>(attendanceResult.Value, request.CompanyId, "Attendance_History_Report", ReportTitle: "Attendance History", null, false);

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;

                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Attendance History Report.");
            #endregion
        }
    }
}
