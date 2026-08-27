using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using QubeFin.Payroll.Persistence.Repositories.ExcelHelpers;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using static QubeFin.Payroll.Persistence.Repositories.ExcelHelpers.ExcelReportHelper;

namespace QubeFin.Payroll.Persistence.Repositories
{
    public interface IReportRepository
    {
        Task<ReportFile> GenerateSSRSAsync(string reportName, string format, Dictionary<string, string> parameters, CancellationToken cancellationToken);
        Task<ReportFile> GenerateExcelAsync(string storedProcedure, Dictionary<string, object?> parameters, Guid companyId, ExcelReportOptions options, CancellationToken cancellationToken);
        Task<ReportFile> GenerateBankSalaryDisbursementExcelAsync(string storedProcedure, Dictionary<string, object?> parameters, Guid companyId, int month, int year, Guid employeeId, CancellationToken cancellationToken);
    }

    public record ReportFile(Stream FileStream, string ContentType, string FileName);
    public class ReportRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory, QubeFinDataContext context) : IReportRepository
    {
        public async Task<ReportFile> GenerateSSRSAsync(string reportName, string format, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            var reportServerHost = configuration["ReportServer:Host"] ?? throw new InvalidOperationException("Report server host is not configured.");
            var reportFolder = configuration["ReportServer:Folder"] ?? throw new InvalidOperationException("Report server folder is not configured.");
            var userId = configuration["ReportServer:UserId"] ?? throw new InvalidOperationException("Report server user is not configured.");
            var password = configuration["ReportServer:Password"] ?? throw new InvalidOperationException("Report server password is not configured.");
            var domain = configuration["ReportServer:Domain"] ?? throw new InvalidOperationException("Report server domain is not configured.");

            var reportFormat = format.ToUpperInvariant() switch
            {
                "PDF" => "PDF",
                "EXCEL" => "EXCELOPENXML",
                "XLSX" => "EXCELOPENXML",
                "WORD" => "WORDOPENXML",
                "DOCX" => "WORDOPENXML",

                _ => throw new ArgumentException($"Unsupported report format: {format}")
            };

            var contentType = reportFormat switch
            {
                "PDF" => "application/pdf",
                "EXCELOPENXML" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "WORDOPENXML" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            var extension = reportFormat switch
            {
                "PDF" => "pdf",
                "EXCELOPENXML" => "xlsx",
                "WORDOPENXML" => "docx",
                _ => "bin"
            };

            var parameterQuery = string.Join("&", parameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
            var reportUrl = $"{reportServerHost}?/" + $"{reportFolder}/{reportName}" + $"&{parameterQuery}" + $"&rs:command=render" + $"&rs:format={reportFormat}";
            var credentialCache = new CredentialCache
            {
                {
                    new Uri(reportServerHost), "NTLM", new NetworkCredential( userId, password, domain)
                }
            };

            using var httpClient = new HttpClient(new HttpClientHandler
            {
                Credentials = credentialCache,
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate,
                PreAuthenticate = true
            });

            var response = await httpClient.GetAsync(reportUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return new ReportFile(stream, contentType, $"{reportName}.{extension}");
        }
        public async Task<ReportFile> GenerateExcelAsync(string storedProcedure, Dictionary<string, object?> parameters, Guid companyId, ExcelReportOptions options, CancellationToken cancellationToken)
        {
            var dataTable = await ReportDataHelper.ExecuteStoredProcedureAsync(configuration.GetConnectionString("DataConnection"), storedProcedure, parameters, cancellationToken);
            var logoBytes = await GetLogoAsync(companyId, cancellationToken);
            var stream = ExcelReportHelper.CreateExcel(dataTable, options, logoBytes);
            return new ReportFile(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{storedProcedure}.xlsx");
        }

        private async Task<byte[]?> GetLogoAsync(Guid companyId, CancellationToken cancellationToken)
        {
            var companyEntity = await context.TblCompanies.AsNoTracking().FirstOrDefaultAsync(m => m.Id == companyId) ?? throw new Exception("Company not found.");
            
            if (string.IsNullOrEmpty(companyEntity.LogoUrl))
            {
                throw new Exception("Please upload the logo for this company.");
            }
            string logoUrl = companyEntity.LogoUrl;

            if (string.IsNullOrWhiteSpace(logoUrl))
                return null;

            var client = httpClientFactory.CreateClient();

            return await client.GetByteArrayAsync(logoUrl, cancellationToken);
        }

        public async Task<ReportFile> GenerateBankSalaryDisbursementExcelAsync(string storedProcedure,Dictionary<string, object?> parameters,Guid companyId, int month, int year, Guid employeeId, CancellationToken cancellationToken)
        {

            var getLoginInfo = await context.TblEmployeeDesignations.Include(m => m.Designation).Include(e => e.Employee).Where(m => m.EmployeeId == employeeId && m.EffectiveTo == null).OrderByDescending(m => m.EffectiveFrom).FirstOrDefaultAsync(cancellationToken);
            string employeeName = getLoginInfo == null ? string.Empty : getLoginInfo.Employee.FullName;
            string designation = getLoginInfo == null ? string.Empty : getLoginInfo.Designation.Name;
            string employeeCode = getLoginInfo == null ? string.Empty : getLoginInfo.Employee.Code;

            var dataTable = await ReportDataHelper.ExecuteStoredProcedureAsync(configuration.GetConnectionString("DataConnection"),storedProcedure,parameters,cancellationToken);

            var logoBytes = await GetLogoAsync(companyId, cancellationToken);
            var stream = ExcelReportHelper.CreateBankSalaryDisbursementExcel(dataTable,logoBytes, month, year, employeeName, designation, employeeCode);

            return new ReportFile(stream,"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",$"{storedProcedure}.xlsx");
        }
    }
}
