using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationUnitsByLocationQuery() : IRequest<Result<List<GetOrganizationUnitsByLocationResponse>>>;
#endregion

#region --- RESPONSE ---
// District and State are null for units that have no District saved yet.
public record GetOrganizationUnitsByLocationResponse(Guid Id, string Name, string OrganizationUnitTypeName,
    Guid? DistrictId, string? DistrictName, Guid? StateId, string? StateName);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitsByLocationQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitsByLocationQuery, Result<List<GetOrganizationUnitsByLocationResponse>>>
{
    public async Task<Result<List<GetOrganizationUnitsByLocationResponse>>> Handle(GetOrganizationUnitsByLocationQuery request, CancellationToken cancellationToken)
    {
        var organizationUnits = await context.TblOrganizationUnits
            .AsNoTracking()
            .OrderBy(m => m.District!.Parent!.Name).ThenBy(m => m.District!.Name).ThenBy(m => m.Name)
            .Select(m => new GetOrganizationUnitsByLocationResponse(
                m.Id,
                m.Name,
                m.OrganizationUnitType.Name,
                m.DistrictId,
                m.District != null ? m.District.Name : null,
                m.District != null ? m.District.ParentId : null,
                m.District != null && m.District.Parent != null ? m.District.Parent.Name : null))
            .ToListAsync(cancellationToken);

        return Result.Ok(organizationUnits);
    }
}
#endregion
