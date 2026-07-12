using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationUnitByIdQuery(Guid Id) : IRequest<Result<GetOrganizationUnitByIdResponse>>;
#endregion

#region --- RESPONSE ---
public record GetOrganizationUnitByIdResponse(Guid Id, string Name, Guid OrganizationUnitTypeId, string OrganizationUnitTypeIcon, string OrganizationUnitTypeName, Guid? ParentId,
    string CreatedBy, DateTime CreatedOn, string? LastModifiedBy, DateTime? LastModifiedOn, IReadOnlyList<OrganizationUnitHierarchyItem> Hierarchy);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitByIdQuery, Result<GetOrganizationUnitByIdResponse>>
{
    public async Task<Result<GetOrganizationUnitByIdResponse>> Handle(GetOrganizationUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var hierarchy = await context.Set<OrganizationUnitHierarchyItem>()
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
                    FROM [Global].[Tbl_OrganizationUnit] au
                    INNER JOIN [Global].[Tbl_OrganizationUnitType] aut
                        ON aut.Id = au.OrganizationUnitTypeId
                    WHERE au.Id = {request.Id}

                    UNION ALL

                    SELECT
                        p.Id,
                        p.Name,
                        pt.Name,
                        pt.Icon,
                        p.ParentId,
                        h.Level + 1
                    FROM [Global].[Tbl_OrganizationUnit] p
                    INNER JOIN [Global].[Tbl_OrganizationUnitType] pt
                        ON pt.Id = p.OrganizationUnitTypeId
                    INNER JOIN Hierarchy h
                        ON h.ParentId = p.Id
                )
                SELECT Id, Name, TypeName, TypeIcon, Level
                FROM Hierarchy
                ORDER BY Level DESC
                ")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var organizationUnit = await context
            .TblOrganizationUnits
            .Include(m => m.OrganizationUnitType)
            .Where(m => m.Id == request.Id)
            .Select(m => new GetOrganizationUnitByIdResponse(m.Id, m.Name, m.OrganizationUnitTypeId, m.OrganizationUnitType.Icon, m.OrganizationUnitType.Name, m.ParentId,
                m.CreatedByNavigation.UserName, m.CreatedOn, m.LastModifiedByNavigation.UserName, m.LastModifiedOn, hierarchy))
            .FirstOrDefaultAsync(cancellationToken);

        if (organizationUnit is null)
        {
            return new RecordNotFoundError($"Organization Unit not found for the given Id");
        }
        return Result.Ok(organizationUnit);
    }
}
#endregion
