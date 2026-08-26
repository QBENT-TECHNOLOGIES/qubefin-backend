using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record CreatePayrollCommand(Guid companyId, Guid? userId) : IRequest<Result<string>>;
    internal sealed class CreatePayrollCommandHandler(IPayrollRepository payrollRepository) : IRequestHandler<CreatePayrollCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreatePayrollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool hasOpenPayroll = await payrollRepository.HasOpenPayrollAsync(request.companyId, cancellationToken);
                if (hasOpenPayroll) return Result.Fail("An unlocked payroll already exists. Please lock the previous payroll before generating a new one.");
                await payrollRepository.CreatePayrollAsync(request.companyId, request.userId, cancellationToken);
                return Result.Ok("Payroll created successfully.");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}