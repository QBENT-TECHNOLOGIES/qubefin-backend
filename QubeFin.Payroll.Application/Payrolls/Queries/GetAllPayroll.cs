using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{   
    public record GetAllPayrollQuery() : IRequest<Result<GetAllPayrollResponse>>;
    public record GetAllPayrollResponse(IEnumerable<PayrollModel> Payrolls);
    internal sealed class GetAllPayrollQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetAllPayrollQuery, Result<GetAllPayrollResponse>>
    {
        public async Task<Result<GetAllPayrollResponse>> Handle(GetAllPayrollQuery request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetAllPayrolls();
            return Result.Ok(new GetAllPayrollResponse(payrolls));
        }
    }
}
