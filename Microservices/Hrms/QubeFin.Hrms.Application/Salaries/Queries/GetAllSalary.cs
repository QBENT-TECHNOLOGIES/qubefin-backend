using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.Queries
{
    public record GetAllSalaryComponentsQuery() : IRequest<Result<GetAllSalaryComponentsResponse>>;
    public record GetAllSalaryComponentsResponse(IEnumerable<SalaryComponent> SalaryComponents);

    internal sealed class GetAllSalaryComponentsQueryHandler(ISalaryComponentRepository salaryRepository) : IRequestHandler<GetAllSalaryComponentsQuery, Result<GetAllSalaryComponentsResponse>>
    {
        public async Task<Result<GetAllSalaryComponentsResponse>> Handle(GetAllSalaryComponentsQuery request, CancellationToken cancellationToken)
        {
            var salaryComponents = await salaryRepository.GetAllSalaryComponents();
            return Result.Ok(new GetAllSalaryComponentsResponse(salaryComponents));
        }
    }
}
