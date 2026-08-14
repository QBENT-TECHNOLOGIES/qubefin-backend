using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Companies.Queries
{   
    public record GetAllCompanyQuery : IRequest<Result<IEnumerable<Company>>>;
    internal sealed class GetAllCompanyQueryHandler(ICompanyRepository companyRepository) : IRequestHandler<GetAllCompanyQuery, Result<IEnumerable<Company>>>
    {
        public async Task<Result<IEnumerable<Company>>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
        {
            var companies = await companyRepository.GetAllCompanies();
            return Result.Ok(companies);
        }
    }
}
 