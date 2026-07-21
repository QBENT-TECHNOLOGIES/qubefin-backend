using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Payroll.Persistence.Repositories;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record CreatePayrollCommand(Guid companyId, Guid? userId) : IRequest<Result<CreatePayrollResponse>>;
    public record CreatePayrollResponse(string message);

    internal sealed class CreatePayrollCommandHandler(IPayrollRepository payrollRepository) : IRequestHandler<CreatePayrollCommand, Result<CreatePayrollResponse>>
    {
        public async Task<Result<CreatePayrollResponse>> Handle(CreatePayrollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool hasOpenPayroll = await payrollRepository.HasOpenPayrollAsync(request.companyId, cancellationToken);
                if (hasOpenPayroll) return new ValidationError("An unlocked payroll already exists. Please lock the previous payroll before generating a new one.");
                await payrollRepository.CreatePayrollAsync(request.companyId, request.userId, cancellationToken);
                return Result.Ok(new CreatePayrollResponse("Payroll created successfully."));
            }
            catch(Exception ex)
            {
                return Result.Fail<CreatePayrollResponse>(ex.Message);
            }

        }
    }
}