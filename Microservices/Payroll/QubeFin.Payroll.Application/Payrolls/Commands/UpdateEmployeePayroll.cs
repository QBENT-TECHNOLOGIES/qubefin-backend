using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Payroll;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record UpdatePayrollComponentsCommand(
        Guid PayrollId,
        List<PayrollComponentModel> EarningHeads,
        List<PayrollComponentModel> DeductionHeads
    ) : IRequest<Result<UpdatePayrollComponentsResponse>>;

    public record UpdatePayrollComponentsResponse(bool IsUpdated);

    internal sealed class UpdateEmployeePayrollCommandHandler(IPayrollRepository payrollRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdatePayrollComponentsCommand, Result<UpdatePayrollComponentsResponse>>
    {
        public async Task<Result<UpdatePayrollComponentsResponse>> Handle(UpdatePayrollComponentsCommand request, CancellationToken cancellationToken)
        {
            var isAvailable = await payrollRepository.IsPayrollAvailableForUpdateAsync(request.PayrollId, cancellationToken);
            if (!isAvailable)
            {
                return new RecordNotFoundError("Employee Payroll not found with the given id");
            }
            var componentsToUpdate = request.EarningHeads.Concat(request.DeductionHeads).ToList();
            await payrollRepository.UpdatePayrollComponentsAsync(request.PayrollId, componentsToUpdate, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdatePayrollComponentsResponse(true));
        }
    }
}