using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{   
    public record GetAllPayrollQuery() : IRequest<Result<IEnumerable<PayrollModel>>>;
    internal sealed class GetAllPayrollQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetAllPayrollQuery, Result<IEnumerable<PayrollModel>>>
    {
        public async Task<Result<IEnumerable<PayrollModel>>> Handle(GetAllPayrollQuery request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetAllPayrolls();
            return Result.Ok(payrolls);
        }
    }
}
