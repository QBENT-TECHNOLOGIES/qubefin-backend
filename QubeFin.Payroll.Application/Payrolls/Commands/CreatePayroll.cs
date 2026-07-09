using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Payroll;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record CreatePayrollCommand(int payrollMonth,
            int payrollYear,
            Guid employeeId,
            Guid organizationUnitId,
            Guid designationId,
            Guid companyId,
            bool isLocked,
            Guid finYearId,
            int? dayCount) : IRequest<Result<CreatePayrollResponse>>;
    public record CreatePayrollResponse(bool Created);
    internal sealed class CreatePayrollCommandHandler(IPayrollRepository payrollRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreatePayrollCommand, Result<CreatePayrollResponse>>
    {
        public async Task<Result<CreatePayrollResponse>> Handle(CreatePayrollCommand request, CancellationToken cancellationToken)
        {
            var payroll = PayrollModel.Create(
                id:Guid.NewGuid(),
                payrollMonth: request.payrollMonth,
                payrollYear: request.payrollYear,
                employeeId: request.employeeId,
                organizationUnitId: request.organizationUnitId,
                designationId: request.designationId,
                companyId: request.companyId,
                isLocked: request.isLocked,
                finYearId: request.finYearId,
                dayCount: request.dayCount
            );
            await payrollRepository.CreatePayroll(payroll);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreatePayrollResponse(true));
        }
    }
}
