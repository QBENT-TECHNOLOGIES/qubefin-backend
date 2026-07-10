using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

#region --- QUERY ---
public record GetAdministrativeUnitByIdQuery(Guid Id) : IRequest<Result<GetAdministrativeUnitByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetAdministrativeUnitByIdResponse(Guid Id, string Name, Guid AdministrativeUnitTypeId, string AdministrativeUnitTypeIcon, string AdministrativeUnitTypeName, Guid? ParentId, bool IsActive, IReadOnlyList<AdministrativeHierarchyItem> Hierarchy);
#endregion

#region --- HANDLER ---
internal sealed class GetAdministrativeUnitByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAdministrativeUnitByIdQuery, Result<GetAdministrativeUnitByIdResponse>>
{
    public async Task<Result<GetAdministrativeUnitByIdResponse>> Handle(GetAdministrativeUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var hierarchy = await context.Set<AdministrativeHierarchyItem>()
            .FromSqlInterpolated($@"
                ;WITH Hierarchy AS
                (
                    SELECT
                        au.Id,
                        au.Name,
                        aut.Name AS TypeName,
                        aut.Icon AS TypeIcon,
                        au.ParentId,
                        0 AS Level
                    FROM [Global].[Tbl_AdministrativeUnit] au
                    INNER JOIN [Global].[Tbl_AdministrativeUnitType] aut
                        ON aut.Id = au.AdministrativeUnitTypeId
                    WHERE au.Id = {request.Id}

                    UNION ALL

                    SELECT
                        p.Id,
                        p.Name,
                        pt.Name,
                        pt.Icon,
                        p.ParentId,
                        h.Level + 1
                    FROM [Global].[Tbl_AdministrativeUnit] p
                    INNER JOIN [Global].[Tbl_AdministrativeUnitType] pt
                        ON pt.Id = p.AdministrativeUnitTypeId
                    INNER JOIN Hierarchy h
                        ON h.ParentId = p.Id
                )
                SELECT Id, Name, TypeName, TypeIcon, Level
                FROM Hierarchy
                ORDER BY Level DESC
                ")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var administrativeUnit = await context
            .TblAdministrativeUnits
            .Include(m => m.AdministrativeUnitType)
            .Where(m => m.Id == request.Id)
            .Select(m => new GetAdministrativeUnitByIdResponse(m.Id, m.Name, m.AdministrativeUnitTypeId, m.AdministrativeUnitType.Icon, m.AdministrativeUnitType.Name, m.ParentId, m.IsActive, hierarchy))
            .FirstOrDefaultAsync(cancellationToken);

        if (administrativeUnit is null)
        {
            return new RecordNotFoundError($"Administrative Unit not found for the given Id");
        }
        return Result.Ok(administrativeUnit);
    }
}
#endregion
