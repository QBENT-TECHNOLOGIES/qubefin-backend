using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Companies.Queries
{   
    public record GetAllCompanyQuery : IRequest<Result<GetAllCompanyResponse>>;
    public record GetAllCompanyResponse(IEnumerable<Company> company);
    internal sealed class GetAllCompanyQueryHandler(ICompanyRepository companyRepository) : IRequestHandler<GetAllCompanyQuery, Result<GetAllCompanyResponse>>
    {
        public async Task<Result<GetAllCompanyResponse>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
        {
            var companies = await companyRepository.GetAllCompanies();
            return Result.Ok(new GetAllCompanyResponse(companies));
        }
    }
}
 