using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Departments.Queries
{
    public record GetAllDepartmentQuery() : IRequest<Result<IEnumerable<Department>>>;
    internal sealed class GetAllDepartmentQueryHandler(IDepartmentRepository departmentRepository) :
        IRequestHandler<GetAllDepartmentQuery, Result<IEnumerable<Department>>>
    {
        public async Task<Result<IEnumerable<Department>>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            var categories = await departmentRepository.GetAllAsync();
            return Result.Ok(categories);
        }
    }
}