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
    Task<List<OrganizationUnit>> GetAllOrganisationUnit(CancellationToken cancellationToken);
    Task AddDesignationAsync(string name, Guid organizationUnitId, Guid postId, Guid roleId, Guid salaryGradeId, Guid userId);
    Task<int> GetNextCodeValAsync(CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, Guid? parentId, Guid? excludedId, CancellationToken cancellationToken);
    Task<bool> IsSelfOrDescendantAsync(Guid candidateParentId, Guid organizationUnitId, CancellationToken cancellationToken);
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

    public async Task<List<OrganizationUnit>> GetAllOrganisationUnit(CancellationToken cancellationToken)
    {
        var organizationUnitEntity = await context.TblOrganizationUnits.AsNoTracking().OrderBy(m => m.CodeVal).ToListAsync(cancellationToken) ?? throw new Exception("No organization found.");
        return organizationUnitEntity?.ToDomain().ToList();
    }
    public async Task<int> GetNextCodeValAsync(CancellationToken cancellationToken)
    {
        var highestCodeVal = await context.TblOrganizationUnits
            .AsNoTracking()
            .MaxAsync(m => (int?)m.CodeVal, cancellationToken);

        return (highestCodeVal ?? 0) + 1;
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? parentId, Guid? excludedId, CancellationToken cancellationToken)
    {
        return await context.TblOrganizationUnits
            .AsNoTracking()
            .AnyAsync(m => m.Name.Trim().ToLower() == name.Trim().ToLower()
                && m.ParentId == parentId
                && (excludedId == null || m.Id != excludedId), cancellationToken);
    }

    public async Task<bool> IsSelfOrDescendantAsync(Guid candidateParentId, Guid organizationUnitId, CancellationToken cancellationToken)
    {
        var parentLookup = await context.TblOrganizationUnits
            .AsNoTracking()
            .Select(m => new { m.Id, m.ParentId })
            .ToDictionaryAsync(m => m.Id, m => m.ParentId, cancellationToken);

        var visitedIds = new HashSet<Guid>();
        var currentId = (Guid?)candidateParentId;

        while (currentId.HasValue && visitedIds.Add(currentId.Value))
        {
            if (currentId.Value == organizationUnitId)
            {
                return true;
            }

            currentId = parentLookup.TryGetValue(currentId.Value, out var parentId) ? parentId : null;
        }

        return false;
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
