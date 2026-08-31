using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static QubeFin.Payroll.Persistence.Repositories.ExcelHelpers.ExcelReportHelper;

namespace QubeFin.Payroll.Application.Payrolls.Report;

#region --- COMMAND ---

public record GenerateEmployeeSalaryRegisterCommand(string StoredProcedure, Dictionary<string, object?> Parameters, Guid companyId, int month, int year) : IRequest<Result<GenerateEmployeeSalaryRegisterResponse>>;

#endregion

#region --- VALIDATOR ---

public class GenerateEmployeeSalaryRegisterCommandValidator : AbstractValidator<GenerateEmployeeSalaryRegisterCommand>
{
    public GenerateEmployeeSalaryRegisterCommandValidator()
    {
        RuleFor(x => x.StoredProcedure).NotEmpty().WithMessage("Stored procedure is required.");
        RuleFor(x => x.Parameters).NotNull().WithMessage("Report parameters are required.");
    }
}

#endregion

#region --- RESPONSE ---

public record GenerateEmployeeSalaryRegisterResponse(Stream FileStream, string ContentType, string FileName);

#endregion

#region --- HANDLER ---

internal sealed class GenerateEmployeeSalaryRegisterCommandHandler(IReportRepository reportRepository) : IRequestHandler<GenerateEmployeeSalaryRegisterCommand, Result<GenerateEmployeeSalaryRegisterResponse>>
{
    public async Task<Result<GenerateEmployeeSalaryRegisterResponse>> Handle(GenerateEmployeeSalaryRegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await reportRepository.GenerateEmployeeSalaryExcelAsync(request.StoredProcedure, request.Parameters, request.companyId, request.month, request.year, cancellationToken);
        return Result.Ok(new GenerateEmployeeSalaryRegisterResponse(result.FileStream, result.ContentType, result.FileName));
    }
}

#endregion
