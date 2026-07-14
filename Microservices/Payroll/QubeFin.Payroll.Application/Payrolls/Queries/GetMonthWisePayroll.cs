using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetMonthwisePayrollSummaryQuery() : IRequest<Result<GetMonthWisePayrollResponse>>;
    public record GetMonthWisePayrollResponse(IEnumerable<MonthwisePayrollData> Payrolls);
    internal class GetMonthWisePayrollQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetMonthwisePayrollSummaryQuery, Result<GetMonthWisePayrollResponse>>
    {
        public async Task<Result<GetMonthWisePayrollResponse>> Handle(GetMonthwisePayrollSummaryQuery request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetMonthwisePayrollSummaryAsync();
            return Result.Ok(new GetMonthWisePayrollResponse(payrolls));
        }
    }
}