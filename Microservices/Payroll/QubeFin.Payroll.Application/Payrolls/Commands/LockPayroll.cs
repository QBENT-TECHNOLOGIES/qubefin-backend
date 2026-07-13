using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Payrolls;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record LockPayrollCommand(int Month, int Year) : IRequest<Result<LockPayrollResponse>>;
    public record LockPayrollResponse(bool lockPayroll);
    internal sealed class LockPayrollCommandHandler(IPayrollRepository payrollRepository, IUnitOfWork unitOfWork) : IRequestHandler<LockPayrollCommand, Result<LockPayrollResponse>>
    {
        public async Task<Result<LockPayrollResponse>> Handle(LockPayrollCommand request, CancellationToken cancellationToken)
        {
            var payrolls = await payrollRepository.GetPayrollsForUpdateAsync(request.Month, request.Year);
            if (payrolls is null || payrolls.Count < 0) return new RecordNotFoundError("No data found");
            foreach (var payroll in payrolls)
            {
                var domainModel = payroll.ToLockingDomain();
                domainModel.Lock();
                domainModel.ApplyToEntity(payroll);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new LockPayrollResponse(true));
        }
    }
}
