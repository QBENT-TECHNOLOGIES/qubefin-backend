using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Commands
{

    #region --- COMMAND ---
    public record CreateEmployeeCommand(Employee employee) : IRequest<Result<CreateEmployeeResponse>>;
    #endregion

    #region --- RESPONSE ---
    public record CreateEmployeeResponse(bool Created);
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateEmployeeCommand, Result<CreateEmployeeResponse>>
    {
        public async Task<Result<CreateEmployeeResponse>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employeeCreate = employeeRepository.CreateEmployee(request.employee);

            

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();

        }
    }
    #endregion
}
