using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Queries
{
    public record GetPayrollByIdQuery(Guid PayrollId) : IRequest<Result<GetPayrollByIdResponse>>;
    public record GetPayrollByIdResponse(PayrollModel Payroll);
    internal sealed class GetPayrollByIdQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetPayrollByIdQuery, Result<GetPayrollByIdResponse>>
    {
        public async Task<Result<GetPayrollByIdResponse>> Handle(GetPayrollByIdQuery request, CancellationToken cancellationToken)
        {
            var payroll = await payrollRepository.GetPayrollById(request.PayrollId);
            if (payroll is null) return new RecordNotFoundError("Payroll not found");
            return Result.Ok(new GetPayrollByIdResponse(payroll));
        }
    }
}
