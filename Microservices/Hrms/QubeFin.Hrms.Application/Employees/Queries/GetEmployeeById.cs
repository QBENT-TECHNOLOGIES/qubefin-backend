using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeByIdQuery(Guid Id) : IRequest<Result<Employee>>;
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeByIdQueryHandler(QubeFinDataContext context, IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeByIdQuery, Result<Employee>>
{
    public async Task<Result<Employee>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        return Result.Ok(employee);
    }
}
#endregion