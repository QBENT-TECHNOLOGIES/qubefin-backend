using FluentResults;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationUnitTreeQuery : IRequest<Result<List<OrganizationUnitTree>>>;
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitTreeQueryHandler(IOrganizationUnitRepository organizationUnitRepository)
    : IRequestHandler<GetOrganizationUnitTreeQuery, Result<List<OrganizationUnitTree>>>
{
    public async Task<Result<List<OrganizationUnitTree>>> Handle(GetOrganizationUnitTreeQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnits = await organizationUnitRepository.GetAllAsync(cancellationToken);
        return Result.Ok(OrganizationUnitTreeBuilder.Build(administrativeUnits));
    }
}
#endregion