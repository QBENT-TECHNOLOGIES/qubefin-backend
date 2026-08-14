using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetPayslipsQuery(Guid employeeId) : IRequest<Result<List<Payslip>>>;
    internal sealed class GetPayslipsQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetPayslipsQuery, Result<List<Payslip>>>
    {
        public async Task<Result<List<Payslip>>> Handle(GetPayslipsQuery request, CancellationToken cancellationToken)
        {
            var payslips = await payrollRepository.GetEmployeePayslipsAsync(request.employeeId);
            return Result.Ok(payslips);
        }
    }
}
