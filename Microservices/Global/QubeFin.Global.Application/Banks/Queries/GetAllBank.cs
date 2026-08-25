using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.Banks.Queries
{
    public record GetAllBankQuery : IRequest<Result<IEnumerable<Bank>>>;

    internal sealed class GetAllBankQueryHandler(IBankRepository bankRepository) : IRequestHandler<GetAllBankQuery, Result<IEnumerable<Bank>>>
    {
        public async Task<Result<IEnumerable<Bank>>> Handle(GetAllBankQuery request, CancellationToken cancellationToken)
        {
            var banks = await bankRepository.GetAllBanks();
            return Result.Ok(banks);
        }
    }
}
