using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeHierarchyQuery(Guid EmployeeId) : IRequest<Result<EmployeeHierarchyNode>>;
#endregion

#region --- VALIDATOR ---
public class GetEmployeeHierarchyQueryValidator : AbstractValidator<GetEmployeeHierarchyQuery>
{
    public GetEmployeeHierarchyQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record EmployeeHierarchyNode(    Guid Id,    string Name,    string? OrganizationUnit,    string? Designation,    List<EmployeeHierarchyNode> Children);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeHierarchyQueryHandler(QubeFinDataContext context) : IRequestHandler<GetEmployeeHierarchyQuery, Result<EmployeeHierarchyNode>>
{
    private static readonly string[] HeadPosts = ["Managing Director", "Divisional Manager", "Area Manager", "Branch Manager"];
    private static readonly Guid[] HeadLevelPostIds = [new("C37C4C1C-9CEA-45FC-97A3-01672E005802")];
    private const int MaxDepth = 50;
    private sealed record Position(Guid EmployeeId, string Name, string Designation, Guid OrganizationUnitId, string OrganizationUnit, bool IsIncharge, bool IsHeadPost, bool IsHeadLevelPost, DateTime EffectiveFrom);

    public async Task<Result<EmployeeHierarchyNode>> Handle(GetEmployeeHierarchyQuery request, CancellationToken cancellationToken)
    {
        var positions = (await context.TblEmployeeDesignations
            .Where(ed => ed.EffectiveTo == null && ed.Employee.IsActive && ed.Employee.IsPayrollActive)
            .Select(ed => new Position(
                ed.EmployeeId,
                ed.Employee.FullName,
                ed.Designation.Name,
                ed.Designation.OrganizationUnitId,
                ed.Designation.OrganizationUnit.Name,
                ed.Designation.IsIncharge,
                HeadPosts.Contains(ed.Designation.Post.Name),
                HeadLevelPostIds.Contains(ed.Designation.PostId),
                ed.EffectiveFrom))
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .GroupBy(p => p.EmployeeId)
            .Select(g => g.OrderByDescending(p => p.EffectiveFrom).First())
            .ToDictionary(p => p.EmployeeId);

        if (!positions.TryGetValue(request.EmployeeId, out var self))
        {
            return Result.Fail("Employee not found or has no current designation.");
        }

        var unitParents = await context.TblOrganizationUnits
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.ParentId, cancellationToken);

        var unitHeads = positions.Values
            .Where(p => p.IsIncharge || p.IsHeadPost)
            .GroupBy(p => p.OrganizationUnitId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.IsIncharge).ThenBy(p => p.EffectiveFrom).First().EmployeeId);

        Guid? NearestHead(Guid? unitId)
        {
            for (var depth = 0; unitId is Guid id && depth < MaxDepth; depth++)
            {
                if (unitHeads.TryGetValue(id, out var headId)) return headId;
                unitId = unitParents.GetValueOrDefault(id);
            }
            return null;
        }

        var reportsTo = positions.Values.ToDictionary(p => p.EmployeeId, p =>
        {
            var isHeadLevel = p.IsHeadLevelPost || unitHeads.GetValueOrDefault(p.OrganizationUnitId) == p.EmployeeId;
            var parentId = isHeadLevel ? NearestHead(unitParents.GetValueOrDefault(p.OrganizationUnitId)) : NearestHead(p.OrganizationUnitId);
            return parentId == p.EmployeeId ? null : parentId;
        });

        var subordinates = positions.Values
            .Where(p => reportsTo[p.EmployeeId] is not null)
            .ToLookup(p => reportsTo[p.EmployeeId]!.Value);

        var visited = new HashSet<Guid>();
        EmployeeHierarchyNode BuildSubtree(Position p) => new(
            p.EmployeeId, p.Name, p.OrganizationUnit, p.Designation,
            visited.Add(p.EmployeeId)
                ? subordinates[p.EmployeeId]
                    .OrderByDescending(c => unitHeads.GetValueOrDefault(c.OrganizationUnitId) == c.EmployeeId)
                    .ThenBy(c => c.OrganizationUnit)
                    .ThenBy(c => c.Name)
                    .Select(BuildSubtree)
                    .ToList()
                : []);

        var node = BuildSubtree(self);
        var current = self.EmployeeId;
        for (var depth = 0; reportsTo[current] is Guid parentId && depth < MaxDepth; depth++)
        {
            var parent = positions[parentId];
            node = new EmployeeHierarchyNode(parent.EmployeeId, parent.Name, parent.OrganizationUnit, parent.Designation, [node]);
            current = parentId;
        }

        return Result.Ok(node);
    }
}
#endregion
