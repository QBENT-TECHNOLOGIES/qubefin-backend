using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;

namespace QubeFin.Payroll.Application.Payrolls.Report;
#region --- COMMAND ---
public record GenerateSSRSReportsCommand(string ReportName, string Format, Dictionary<string, string> Parameters) : IRequest<Result<GenerateSSRSReportsResponse>>;

#endregion

#region --- VALIDATOR ---

public class GenerateSSRSReportsCommandValidator : AbstractValidator<GenerateSSRSReportsCommand>
{
    public GenerateSSRSReportsCommandValidator()
    {
        RuleFor(x => x.ReportName).NotEmpty().WithMessage("Report name is required.");
        RuleFor(x => x.Format).NotEmpty().WithMessage("Report format is required.");
        RuleFor(x => x.Parameters).NotNull().WithMessage("Report parameters are required.");
    }
}

#endregion

#region --- RESPONSE ---

public record GenerateSSRSReportsResponse(Stream FileStream, string ContentType, string FileName);

#endregion

#region --- HANDLER ---

internal sealed class GenerateSSRSReportsCommandHandler(IReportRepository reportRepository) : IRequestHandler<GenerateSSRSReportsCommand, Result<GenerateSSRSReportsResponse>>
{
    public async Task<Result<GenerateSSRSReportsResponse>> Handle(GenerateSSRSReportsCommand request, CancellationToken cancellationToken)
    {
        var result = await reportRepository.GenerateSSRSAsync(request.ReportName, request.Format, request.Parameters, cancellationToken);
        return Result.Ok(new GenerateSSRSReportsResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion