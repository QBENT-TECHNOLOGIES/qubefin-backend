using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using static QubeFin.Payroll.Persistence.Repositories.ExcelHelpers.ExcelReportHelper;

namespace QubeFin.Payroll.Application.Payrolls.Report;

#region --- COMMAND ---

public record GenerateNPOIReportsCommand(string StoredProcedure, Dictionary<string, object?> Parameters, string? Header, string? SubHeader, bool ShowCompanyHeader, Guid companyId) : IRequest<Result<GenerateNPOIReportsResponse>>;

#endregion

#region --- VALIDATOR ---

public class GenerateNPOIReportsCommandValidator : AbstractValidator<GenerateNPOIReportsCommand>
{
    public GenerateNPOIReportsCommandValidator()
    {
        RuleFor(x => x.StoredProcedure).NotEmpty().WithMessage("Stored procedure is required.");
        RuleFor(x => x.Parameters).NotNull().WithMessage("Report parameters are required.");
    }
}

#endregion

#region --- RESPONSE ---

public record GenerateNPOIReportsResponse(Stream FileStream, string ContentType, string FileName);

#endregion

#region --- HANDLER ---

internal sealed class GenerateNPOIReportsCommandHandler(IReportRepository reportRepository) : IRequestHandler<GenerateNPOIReportsCommand, Result<GenerateNPOIReportsResponse>>
{
    public async Task<Result<GenerateNPOIReportsResponse>> Handle(GenerateNPOIReportsCommand request, CancellationToken cancellationToken)
    {
        var options = new ExcelReportOptions(ShowCompanyHeader: request.ShowCompanyHeader, ReportTitle: request.Header, SubHeader: request.SubHeader);
        var result = await reportRepository.GenerateExcelAsync(request.StoredProcedure, request.Parameters, request.companyId, options, cancellationToken);
        return Result.Ok(new GenerateNPOIReportsResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion
