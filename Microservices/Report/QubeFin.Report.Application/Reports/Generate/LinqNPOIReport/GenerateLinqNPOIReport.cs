using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Report.Application.Reports.Generate.NPOIReports;
using QubeFin.Report.Persistence.Repositories;
using static QubeFin.Report.Persistence.Repositories.ExcelHelpers.ExcelReportHelper;

namespace QubeFin.Report.Application.Reports.Generate.LinqNPOIReport;

#region --- COMMAND ---

public record GenerateLinqNPOIReportCommand<T>(IEnumerable<T> Data, Guid CompanyId, string FileName, string? ReportTitle = null, string? SubHeader = null, bool ShowCompanyHeader = false) : IRequest<Result<GenerateNPOIReportsResponse>>;

#endregion

#region --- VALIDATOR ---

public class GenerateLinqNPOIReportCommandValidator<T> : AbstractValidator<GenerateLinqNPOIReportCommand<T>>
{
    public GenerateLinqNPOIReportCommandValidator()
    {
        RuleFor(x => x.Data).NotNull().WithMessage("Report data is required.");
        RuleFor(x => x.CompanyId).NotEmpty().WithMessage("Company id is required.");
        RuleFor(x => x.FileName).NotEmpty().WithMessage("File name is required.");
    }
}

#endregion

#region --- HANDLER ---

internal sealed class GenerateLinqNPOIReportHandler<T>(IReportRepository reportRepository)
    : IRequestHandler<GenerateLinqNPOIReportCommand<T>, Result<GenerateNPOIReportsResponse>>
{
    public async Task<Result<GenerateNPOIReportsResponse>> Handle(GenerateLinqNPOIReportCommand<T> request, CancellationToken cancellationToken)
    {
        var options = new ExcelReportOptions(ShowCompanyHeader: request.ShowCompanyHeader, ReportTitle: request.ReportTitle, SubHeader: request.SubHeader);
        var result = await reportRepository.GenerateExcelFromLINQAsync(request.Data, request.CompanyId, options, request.FileName, cancellationToken);

        return Result.Ok(new GenerateNPOIReportsResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion