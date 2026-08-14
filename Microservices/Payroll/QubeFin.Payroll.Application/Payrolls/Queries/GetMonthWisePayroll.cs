using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetMonthwisePayrollSummaryQuery() : IRequest<Result<IEnumerable<MonthwisePayrollData>>>;
    internal class GetMonthWisePayrollQueryHandler(IPayrollRepository payrollRepository) : 
        IRequestHandler<GetMonthwisePayrollSummaryQuery, Result<IEnumerable<MonthwisePayrollData>>>
    {
        public async Task<Result<IEnumerable<MonthwisePayrollData>>> Handle(GetMonthwisePayrollSummaryQuery request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetMonthwisePayrollSummaryAsync();
            return Result.Ok(payrolls);
        }
    }
}