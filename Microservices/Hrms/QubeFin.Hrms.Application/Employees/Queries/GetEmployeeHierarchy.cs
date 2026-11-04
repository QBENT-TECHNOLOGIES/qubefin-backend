using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeHierarchyQuery(Guid EmployeeId) : IRequest<Result<EmployeeHierarchyResponse?>>;
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
// Null when the employee has no current designation
public record EmployeeHierarchyResponse(Guid EmployeeId, EmployeeHierarchyNode Root);
// One member normally; several when a unit has no Incharge and more than one head post holder (shared team head).
// No members and VacantUnit set when a unit has nobody to head it (e.g. an Area without an Area Manager).
public record EmployeeHierarchyNode(List<EmployeeHierarchyMember> Members, List<EmployeeHierarchyNode> Children, string? VacantUnit = null);
public record EmployeeHierarchyMember(Guid Id, string Name, string? OrganizationUnit, string? Designation);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeHierarchyQueryHandler(QubeFinDataContext context) : IRequestHandler<GetEmployeeHierarchyQuery, Result<EmployeeHierarchyResponse?>>
{
    private static readonly string[] HeadPosts = ["Managing Director", "Divisional Manager", "Area Manager", "Branch Manager"];
    private const int MaxDepth = 50;
    private sealed record Position(Guid EmployeeId, string Name, string Designation, Guid OrganizationUnitId, string OrganizationUnit, bool IsIncharge, bool IsHeadPost, DateTime EffectiveFrom);

    public async Task<Result<EmployeeHierarchyResponse?>> Handle(GetEmployeeHierarchyQuery request, CancellationToken cancellationToken)
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
                ed.EffectiveFrom))
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .GroupBy(p => p.EmployeeId)
            .Select(g => g.OrderByDescending(p => p.EffectiveFrom).First())
            .ToDictionary(p => p.EmployeeId);

        if (!positions.TryGetValue(request.EmployeeId, out var self))
        {
            return Result.Ok<EmployeeHierarchyResponse?>(null);
        }

        var units = await context.TblOrganizationUnits
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => (u.ParentId, u.Name), cancellationToken);

        // Team head of a unit is its Incharge; without one, every head post holder shares the head
        var unitHeads = positions.Values
            .GroupBy(p => p.OrganizationUnitId)
            .Select(g => (UnitId: g.Key, Heads: g.Any(p => p.IsIncharge)
                ? g.Where(p => p.IsIncharge).OrderBy(p => p.EffectiveFrom).Take(1).ToList()
                : g.Where(p => p.IsHeadPost).OrderBy(p => p.EffectiveFrom).ToList()))
            .Where(u => u.Heads.Count > 0)
            .ToDictionary(u => u.UnitId, u => u.Heads);
        var headIds = unitHeads.Values.SelectMany(h => h).Select(p => p.EmployeeId).ToHashSet();

        Guid? ParentUnit(Guid unitId) => units.TryGetValue(unitId, out var unit) ? unit.ParentId : null;

        // Every unit with staff in or below it gets a node, so a unit without a head still shows as vacant
        var activeUnits = new HashSet<Guid>();
        foreach (var p in positions.Values)
        {
            for (Guid? unitId = p.OrganizationUnitId; unitId is Guid id && activeUnits.Count <= units.Count && activeUnits.Add(id);)
            {
                unitId = ParentUnit(id);
            }
        }

        // Unit nodes (its heads, or vacant) are keyed by the unit id; everyone else by their employee id
        Guid NodeKey(Position p) => headIds.Contains(p.EmployeeId) ? p.OrganizationUnitId : p.EmployeeId;
        var staff = positions.Values.Where(p => !headIds.Contains(p.EmployeeId)).ToDictionary(p => p.EmployeeId);

        var reportsTo = activeUnits.ToDictionary(u => u, ParentUnit);   // unit -> unit above
        foreach (var p in staff.Values) reportsTo[p.EmployeeId] = p.OrganizationUnitId;   // staff -> own unit

        var subordinates = reportsTo
            .Where(r => r.Value is not null)
            .ToLookup(r => r.Value!.Value, r => r.Key);

        List<EmployeeHierarchyMember> Members(Guid key) =>
            (unitHeads.TryGetValue(key, out var heads) ? heads : staff.TryGetValue(key, out var p) ? [p] : [])
            .Select(m => new EmployeeHierarchyMember(m.EmployeeId, m.Name, m.OrganizationUnit, m.Designation))
            .ToList();

        EmployeeHierarchyNode Node(Guid key, List<EmployeeHierarchyNode> children) =>
            activeUnits.Contains(key) && !unitHeads.ContainsKey(key)
                ? new([], children, units[key].Name)
                : new(Members(key), children);

        string SortName(Guid key) => activeUnits.Contains(key) ? units[key].Name : staff[key].Name;

        var visited = new HashSet<Guid>();
        EmployeeHierarchyNode BuildSubtree(Guid key) => Node(key,
            visited.Add(key)
                ? subordinates[key]
                    .OrderByDescending(activeUnits.Contains)
                    .ThenBy(SortName)
                    .Select(BuildSubtree)
                    .ToList()
                : []);

        var current = NodeKey(self);
        var node = BuildSubtree(current);
        for (var depth = 0; reportsTo[current] is Guid parentKey && depth < MaxDepth; depth++)
        {
            node = Node(parentKey, [node]);
            current = parentKey;
        }

        return Result.Ok<EmployeeHierarchyResponse?>(new EmployeeHierarchyResponse(self.EmployeeId, node));
    }
}
#endregion
