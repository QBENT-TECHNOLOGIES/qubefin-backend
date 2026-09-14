using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Report.Persistence.Repositories;

namespace QubeFin.Report.Application.Reports.Generate.CustomNPOIReports;

#region --- COMMAND ---

public record GenerateSalaryDisbursementSheetCommand(string StoredProcedure, Dictionary<string, object?> Parameters, int month, int year, Guid companyId, Guid employeeId) : IRequest<Result<GenerateSalaryDisbursementSheetResponse>>;

#endregion

#region --- VALIDATOR ---

public class GenerateSalaryDisbursementSheetCommandValidator : AbstractValidator<GenerateSalaryDisbursementSheetCommand>
{
    public GenerateSalaryDisbursementSheetCommandValidator()
    {
        RuleFor(x => x.StoredProcedure).NotEmpty().WithMessage("Stored procedure is required.");
        RuleFor(x => x.Parameters).NotNull().WithMessage("Report parameters are required.");
    }
}

#endregion

#region --- RESPONSE ---

public record GenerateSalaryDisbursementSheetResponse(Stream FileStream, string ContentType, string FileName);

#endregion

#region --- HANDLER ---

internal sealed class GenerateSalaryDisbursementSheetCommandHandler(IReportRepository reportRepository, ISender sender) : IRequestHandler<GenerateSalaryDisbursementSheetCommand, Result<GenerateSalaryDisbursementSheetResponse>>
{
    public async Task<Result<GenerateSalaryDisbursementSheetResponse>> Handle(GenerateSalaryDisbursementSheetCommand request, CancellationToken cancellationToken)
    {
        var result = await reportRepository.GenerateBankSalaryDisbursementExcelAsync(request.StoredProcedure, request.Parameters, request.companyId, request.month, request.year, request.employeeId, cancellationToken);
        return Result.Ok(new GenerateSalaryDisbursementSheetResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion
