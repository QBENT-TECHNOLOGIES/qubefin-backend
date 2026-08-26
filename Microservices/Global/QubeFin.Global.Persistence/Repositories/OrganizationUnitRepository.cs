using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories;

public interface IOrganizationUnitRepository
{
    Task<IEnumerable<OrganizationUnitTree>> GetAllAsync(CancellationToken cancellationToken);
    Task<OrganizationUnit?> GetByIdAsync(Guid id);
    Task AddAsync(OrganizationUnit organizationUnit);
    void Update(OrganizationUnit organizationUnit);
    Task AddDesignationAsync(string name, Guid organizationUnitId, Guid postId, Guid roleId, Guid salaryGradeId, Guid userId);
}

internal class OrganizationUnitRepository(QubeFinDataContext context) : IOrganizationUnitRepository
{
    public async Task AddAsync(OrganizationUnit organizationUnit)
    {
        await context.TblOrganizationUnits.AddAsync(organizationUnit.ToEntity());
    }

    public async Task<IEnumerable<OrganizationUnitTree>> GetAllAsync(CancellationToken cancellationToken)
    {
        var organizationUnitEntities = await context
            .TblOrganizationUnits
            .Include(m => m.OrganizationUnitType)
            .AsNoTracking()
            .Select(m => new OrganizationUnitTree
            {
                Id = m.Id,
                OrganizationUnitTypeId = m.OrganizationUnitTypeId,
                OrganizationUnitTypeIcon = m.OrganizationUnitType.Icon,
                OrganizationUnitTypeName = m.OrganizationUnitType.Name,
                Name = m.Name,
                ParentId = m.ParentId
            })
            .ToListAsync(cancellationToken);

        return organizationUnitEntities;
    }
    public async Task<OrganizationUnit?> GetByIdAsync(Guid id)
    {
        var organizationUnitEntity = await context.TblOrganizationUnits.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return organizationUnitEntity?.ToDomain();
    }

    public void Update(OrganizationUnit organizationUnit)
    {
        context.TblOrganizationUnits.Update(organizationUnit.ToEntity());
    }
    public async Task AddDesignationAsync(string name, Guid organizationUnitId, Guid postId, Guid roleId, Guid salaryGradeId, Guid userId)
    {
        var existingDesignation = await context.TblDesignations
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Name.Trim().ToLower() == name.Trim().ToLower() && d.OrganizationUnitId == organizationUnitId);
        if (existingDesignation != null)
        {
            throw new InvalidOperationException($"Designation {name} already exists under the specified organization unit.");
        }

        var designation = new TblDesignation
        {
            Id = Guid.NewGuid(),
            Name = name,
            OrganizationUnitId = organizationUnitId,
            PostId = postId,
            IsActive = true,
            CreatedBy = userId,
            CreatedOn = DateTime.UtcNow,
            TblDesignationRoles = new List<TblDesignationRole>
            {
                new TblDesignationRole
                {
                    Id = Guid.NewGuid(),
                    RoleId = roleId,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                }
            },
            TblDesignationGradeMappings = new List<TblDesignationGradeMapping>
            {
                new TblDesignationGradeMapping
                {
                    Id = Guid.NewGuid(),
                    GradeId = salaryGradeId,
                    IsActive = true
                }
            }
        };
        await context.TblDesignations.AddAsync(designation);
    }
}
