using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetPayslipsQuery(Guid employeeId) : IRequest<Result<GetPayslipsResponse>>;
    public record GetPayslipsResponse(IEnumerable<Payslip> payslips);
    internal sealed class GetPayslipsQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetPayslipsQuery, Result<GetPayslipsResponse>>
    {
        public async Task<Result<GetPayslipsResponse>> Handle(GetPayslipsQuery request, CancellationToken cancellationToken)
        {
            var payslips = await payrollRepository.GetEmployeePayslipsAsync(request.employeeId);
            return Result.Ok(new GetPayslipsResponse(payslips));
        }
    }
}
