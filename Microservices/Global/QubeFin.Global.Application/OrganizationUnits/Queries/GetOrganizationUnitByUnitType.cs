using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationUnitByUnitTypeQuery(Guid UnitTypeId) : IRequest<Result<List<GetOrganizationUnitByUnitTypeResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetOrganizationUnitByUnitTypeResponse(Guid Id, string Name);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitByUnitTypeQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitByUnitTypeQuery, Result<List<GetOrganizationUnitByUnitTypeResponse>>>
{
    public async Task<Result<List<GetOrganizationUnitByUnitTypeResponse>>> Handle(GetOrganizationUnitByUnitTypeQuery request, CancellationToken cancellationToken)
    {
        var organizationUnitEntities = await context
            .TblOrganizationUnits
            .Where(m => m.OrganizationUnitTypeId == request.UnitTypeId)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        var organizationUnits = organizationUnitEntities.Select(m => new GetOrganizationUnitByUnitTypeResponse(m.Id, m.Name)).ToList();
        return Result.Ok(organizationUnits);
    }
}
#endregion
