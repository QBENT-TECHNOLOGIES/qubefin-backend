using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;
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

        var users = await context.TblUsers
            .Where(u => u.Id == employee.CreatedBy || u.Id == employee.LastModifiedBy)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        employee.SetAuditInfo(new AuditInfo
        {
            CreatedBy = users.FirstOrDefault(u => u.Id == employee.CreatedBy)?.UserName ?? string.Empty,
            CreatedOn = employee.CreatedDate,
            LastModifiedBy = users.FirstOrDefault(u => u.Id == employee.LastModifiedBy)?.UserName ?? string.Empty,
            LastModifiedOn = employee.LastModifiedOn
        });

        return Result.Ok(employee);
    }
}
#endregion