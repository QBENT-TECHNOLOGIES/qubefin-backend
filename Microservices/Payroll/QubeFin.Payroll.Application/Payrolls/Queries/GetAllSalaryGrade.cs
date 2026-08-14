using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;

namespace QubeFin.Payroll.Application.Payrolls.Queries;

public record GetAllSalaryGradeQuery() : IRequest<Result<List<GetAllSalaryGradeResponse>>>;

public record GetAllSalaryGradeResponse(Guid Id, string Name, string Code, bool IsActive);
internal sealed class GetAllSalaryGradeQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetAllSalaryGradeQuery, Result<List<GetAllSalaryGradeResponse>>>
{
    public async Task<Result<List<GetAllSalaryGradeResponse>>> Handle(GetAllSalaryGradeQuery request, CancellationToken cancellationToken)
    {
        var salaryGrade = await payrollRepository.GetAllSalaryGrade();
        return Result.Ok(salaryGrade.Select(m => new GetAllSalaryGradeResponse(m.Id, m.Name, m.Code, m.IsActive)).ToList());
    }
}
