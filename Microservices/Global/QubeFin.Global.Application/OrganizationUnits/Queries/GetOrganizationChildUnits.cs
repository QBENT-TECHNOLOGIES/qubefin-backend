using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationChildUnitsQuery(Guid? ParentId) : IRequest<Result<List<GetOrganizationChildUnitsResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetOrganizationChildUnitsResponse(Guid Id, string Name, string TypeIcon);
#endregion  

#region --- HANDLER ---
internal sealed class GetOrganizationChildUnitsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationChildUnitsQuery, Result<List<GetOrganizationChildUnitsResponse>>>
{
    public async Task<Result<List<GetOrganizationChildUnitsResponse>>> Handle(GetOrganizationChildUnitsQuery request, CancellationToken cancellationToken)
    {
        var organizationUnits = await context
            .TblOrganizationUnits
            .Include(m => m.OrganizationUnitType)
            .Where(m => m.ParentId == request.ParentId)
            .OrderBy(m => m.Name)
            .Select(m => new GetOrganizationChildUnitsResponse(m.Id, m.Name, m.OrganizationUnitType.Icon))
            .ToListAsync(cancellationToken);

        return Result.Ok(organizationUnits);
    }
}
#endregion