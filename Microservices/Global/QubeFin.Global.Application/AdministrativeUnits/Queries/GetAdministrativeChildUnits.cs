using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

#region --- QUERY ---
public record GetAdministrativeChildUnitsQuery(Guid? ParentId) : IRequest<Result<List<GetAdministrativeChildUnitsResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetAdministrativeChildUnitsResponse(Guid Id, string Name, string Category, string CategoryIcon);
#endregion  

#region --- HANDLER ---
internal sealed class GetAdministrativeChildUnitsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAdministrativeChildUnitsQuery, Result<List<GetAdministrativeChildUnitsResponse>>>
{
    public async Task<Result<List<GetAdministrativeChildUnitsResponse>>> Handle(GetAdministrativeChildUnitsQuery request, CancellationToken cancellationToken)
    {
        var administrativeUnits = await context
            .TblAdministrativeUnits
            .Include(m => m.AdministrativeUnitType)
            .Where(m => m.ParentId == request.ParentId)
            .OrderBy(m => m.Name)
            .Select(m => new GetAdministrativeChildUnitsResponse(m.Id, m.Name, m.AdministrativeUnitType.Category, m.AdministrativeUnitType.Icon))
            .ToListAsync(cancellationToken);

        return Result.Ok(administrativeUnits);
    }
}
#endregion
