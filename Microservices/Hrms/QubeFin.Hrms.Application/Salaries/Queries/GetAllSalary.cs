using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.Queries
{
    public record GetAllSalaryComponentsQuery() : IRequest<Result<IEnumerable<SalaryComponent>>>;
    internal sealed class GetAllSalaryComponentsQueryHandler(ISalaryComponentRepository salaryRepository) :
        IRequestHandler<GetAllSalaryComponentsQuery, Result<IEnumerable<SalaryComponent>>>
    {
        public async Task<Result<IEnumerable<SalaryComponent>>> Handle(GetAllSalaryComponentsQuery request, CancellationToken cancellationToken)
        {
            var salaryComponents = await salaryRepository.GetAllSalaryComponents();
            return Result.Ok(salaryComponents);
        }
    }
}
