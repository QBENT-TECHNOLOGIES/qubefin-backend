using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Salaries.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Hrms.Application.Salaries.Queries
{
    public record GetSalaryComponentByIdQuery(Guid Id) : IRequest<Result<GetSalaryComponentByIdResponse>>;
    public record GetSalaryComponentByIdResponse(SalaryComponentResponse SalaryComponent);
    internal sealed class GetSalaryByIdQueryHandler(ISalaryComponentRepository salaryRepository, QubeFinDataContext context) : IRequestHandler<GetSalaryComponentByIdQuery, Result<GetSalaryComponentByIdResponse>>
    {
        public async Task<Result<GetSalaryComponentByIdResponse>> Handle(GetSalaryComponentByIdQuery request, CancellationToken cancellationToken)
        {
            var salaryComponent = await salaryRepository.GetSalaryComponentById(request.Id);
            if (salaryComponent is null) return new RecordNotFoundError("Salary Component not found");

            var users = await context.TblUsers.Where(u => u.Id == salaryComponent.CreatedBy || u.Id == salaryComponent.LastModifiedBy).AsNoTracking().ToListAsync(cancellationToken);

            var response = new SalaryComponentResponse(
                salaryComponent.Id,
                salaryComponent.Name,
                salaryComponent.Code,
                salaryComponent.CategoryId,
                salaryComponent.CategoryName,
                salaryComponent.IsTaxable,
                salaryComponent.IsPfapplicable,
                salaryComponent.IsEsiapplicable,
                salaryComponent.IsCtccomponent,
                salaryComponent.IsActive,
                salaryComponent.DisplayOrder,
                salaryComponent.CreatedOn,
                salaryComponent.CreatedBy,
                salaryComponent.LastModifiedOn,
                salaryComponent.LastModifiedBy);

            response.AuditInfo = new AuditInfo
            {
                CreatedBy = users.FirstOrDefault(u => u.Id == salaryComponent.CreatedBy)?.UserName ?? string.Empty,
                CreatedOn = salaryComponent.CreatedOn,
                LastModifiedBy = users.FirstOrDefault(u => u.Id == salaryComponent.LastModifiedBy)?.UserName ?? string.Empty,
                LastModifiedOn = salaryComponent.LastModifiedOn
            };

            return Result.Ok(new GetSalaryComponentByIdResponse(response));
        }
    }
}
