using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetMonthlyPayrollQuery(int PayrollMonth, int PayrollYear) : IRequest<Result<MonthlyPayroll>>;
    internal sealed class GetMonthlyPayrollQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetMonthlyPayrollQuery, Result<MonthlyPayroll>>
    {
        public async Task<Result<MonthlyPayroll>> Handle(GetMonthlyPayrollQuery request, CancellationToken cancellationToken)
        {
            var payroll = await payrollRepository.GetMonthlyPayrollAsync(request.PayrollMonth, request.PayrollYear);
            return Result.Ok(payroll);
        }
    }
}
