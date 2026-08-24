using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetMonthwisePayrollSummaryQuery(Guid? companyId, int? payrollMonth, int payrollYear) : IRequest<Result<IEnumerable<MonthwisePayrollData>>>;
    internal class GetMonthWisePayrollQueryHandler(IPayrollRepository payrollRepository) : 
        IRequestHandler<GetMonthwisePayrollSummaryQuery, Result<IEnumerable<MonthwisePayrollData>>>
    {
        public async Task<Result<IEnumerable<MonthwisePayrollData>>> Handle(GetMonthwisePayrollSummaryQuery request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetMonthwisePayrollSummaryAsync(request.companyId, request.payrollMonth, request.payrollYear);
            return Result.Ok(payrolls);
        }
    }
}