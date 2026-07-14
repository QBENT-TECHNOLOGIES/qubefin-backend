using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetMonthlyPayrollQuery(int PayrollMonth, int PayrollYear) : IRequest<Result<GetMonthlyPayrollResponse>>;
    public record GetMonthlyPayrollResponse(MonthlyPayroll Payroll);

    internal sealed class GetMonthlyPayrollQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetMonthlyPayrollQuery, Result<GetMonthlyPayrollResponse>>
    {
        public async Task<Result<GetMonthlyPayrollResponse>> Handle(GetMonthlyPayrollQuery request, CancellationToken cancellationToken)
        {
            var payroll = await payrollRepository.GetMonthlyPayrollAsync(request.PayrollMonth, request.PayrollYear);
            return Result.Ok(new GetMonthlyPayrollResponse(payroll));
        }
    }
}
