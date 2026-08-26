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
    string CreatedBy, DateTime CreatedOn, string? LastModifiedBy, DateTime? LastModifiedOn, IReadOnlyList<OrganizationUnitHierarchyItem> Hierarchy,
    IReadOnlyList<OragnizationDesignations> Designations);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitByIdQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitByIdQuery, Result<GetOrganizationUnitByIdResponse>>
{
    public async Task<Result<GetOrganizationUnitByIdResponse>> Handle(GetOrganizationUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var organizationUnit = await context
         .TblOrganizationUnits
         .Include(m => m.OrganizationUnitType).Include(m => m.CreatedByNavigation).Include(m => m.LastModifiedByNavigation)
         .Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (organizationUnit is null)
        {
            return new RecordNotFoundError($"Organization Unit not found for the given Id");
        }

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

        var designations = await context.TblDesignations
            .Where(d => d.OrganizationUnitId == request.Id)
            .Select(d => new OragnizationDesignations
            {
                Id = d.Id,
                Name = d.Name,
                PostId = d.PostId,
                PostName = d.Post.Name,
                IsActive = d.IsActive,
                RoleId = d.TblDesignationRoles.Select(r => (Guid?)r.RoleId).FirstOrDefault(),
                RoleName = d.TblDesignationRoles.Select(r => r.Role.Name).FirstOrDefault(),
                GradeId = d.TblDesignationGradeMappings.Select(g => (Guid?)g.GradeId).FirstOrDefault(),
                GradeName = d.TblDesignationGradeMappings.Select(g => g.Grade.Name).FirstOrDefault(),
            })
            .OrderBy(d => d.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetOrganizationUnitByIdResponse(
            organizationUnit.Id, organizationUnit.Name,
            organizationUnit.OrganizationUnitTypeId, organizationUnit.OrganizationUnitType.Icon,
            organizationUnit.OrganizationUnitType.Name, organizationUnit.ParentId,
            organizationUnit.CreatedByNavigation.UserName, organizationUnit.CreatedOn,
            organizationUnit.LastModifiedByNavigation?.UserName, organizationUnit.LastModifiedOn, hierarchy, designations);

        return Result.Ok(result);
    }
}
#endregion
