using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Departments.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Hrms.Application.Departments.Queries;

public record GetDepartmentByIdQuery(Guid Id)
    : IRequest<Result<DepartmentResponse>>;

internal sealed class GetDepartmentByIdQueryHandler(
    IDepartmentRepository departmentRepository,
    QubeFinDataContext context)
    : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentResponse>>
{
    public async Task<Result<DepartmentResponse>> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetByIdAsync(request.Id);

        if (department is null)
            return new RecordNotFoundError("Department not found.");
        var hodEmployeeName = await context.TblEmployees
            .Where(e => e.Id == department.HodEmployeeId)
            .Select(e => e.FullName) 
            .FirstOrDefaultAsync(cancellationToken);

        var users = await context.TblUsers
            .Where(u =>
                u.Id == department.CreatedBy ||
                u.Id == department.LastModifiedBy)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            IsActive = department.IsActive,
            HodEmployeeId = department.HodEmployeeId,
            HodEmployeeName = hodEmployeeName,

            AuditInfo = new AuditInfo
            {
                CreatedBy = users
                    .FirstOrDefault(u => u.Id == department.CreatedBy)
                    ?.UserName ?? string.Empty,

                CreatedOn = department.CreatedOn,

                LastModifiedBy = users
                    .FirstOrDefault(u => u.Id == department.LastModifiedBy)
                    ?.UserName ?? string.Empty,

                LastModifiedOn = department.LastModifiedOn
            }
        };

        return Result.Ok(response);
    }
}