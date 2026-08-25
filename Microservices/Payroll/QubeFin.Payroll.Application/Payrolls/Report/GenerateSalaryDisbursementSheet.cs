using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;

namespace QubeFin.Payroll.Application.Payrolls.Report;

internal class GenerateSalaryDisbursementSheet
{
}



#region --- COMMAND ---

public record GenerateSalaryDisbursementSheetCommand(string StoredProcedure, Dictionary<string, object?> Parameters, Guid companyId) : IRequest<Result<GenerateSalaryDisbursementSheetResponse>>;

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

internal sealed class GenerateSalaryDisbursementSheetCommandHandler(IReportRepository reportRepository) : IRequestHandler<GenerateSalaryDisbursementSheetCommand, Result<GenerateSalaryDisbursementSheetResponse>>
{
    public async Task<Result<GenerateSalaryDisbursementSheetResponse>> Handle(GenerateSalaryDisbursementSheetCommand request, CancellationToken cancellationToken)
    {
        var result = await reportRepository.GenerateBankSalaryDisbursementExcelAsync(request.StoredProcedure, request.Parameters, request.companyId, cancellationToken);
        return Result.Ok(new GenerateSalaryDisbursementSheetResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion
