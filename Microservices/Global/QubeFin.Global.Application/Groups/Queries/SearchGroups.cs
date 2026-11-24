using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Application.Groups.Models;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Groups.Queries;

#region --- QUERY ---
public record SearchGroupsQuery(string? SearchText, string? Status, string? SortOn, string? SortDirection, int PageIndex, int PageSize, Guid EmployeeId)
    : IRequest<Result<GroupSearchResponse>>;
#endregion

#region --- VALIDATOR ---
public class SearchGroupsQueryValidator : AbstractValidator<SearchGroupsQuery>
{
    public SearchGroupsQueryValidator()
    {
        RuleFor(v => v.PageIndex).GreaterThanOrEqualTo(0);
        RuleFor(v => v.PageSize).InclusiveBetween(1, 100);
    }
}
#endregion

#region --- HANDLER ---
internal sealed class SearchGroupsQueryHandler(QubeFinDataContext context) : IRequestHandler<SearchGroupsQuery, Result<GroupSearchResponse>>
{
    public async Task<Result<GroupSearchResponse>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        var visibility = await GroupAccess.GetVisibilityAsync(context, request.EmployeeId, cancellationToken);
        var ownIds = visibility.OwnDesignationIds;
        var branchCodes = visibility.BranchCodes.Select(c => (int?)c).ToList();

        var query = context.TblGroups.AsNoTracking()
            .Where(g => (g.DesignationId != null && ownIds.Contains(g.DesignationId.Value)) || branchCodes.Contains(g.BranchCode));

        var text = (request.SearchText ?? string.Empty).Trim();
        if (text.Length > 0)
            query = query.Where(g => g.Name.Contains(text) || (g.Code != null && g.Code.Contains(text)));

        var status = (request.Status ?? string.Empty).Trim();
        if (status.Length > 0)
            query = query.Where(g => g.CurrentStatus == status);

        var desc = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        query = (request.SortOn ?? string.Empty).ToLower() switch
        {
            "name" => desc ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
            "code" => desc ? query.OrderByDescending(g => g.Code) : query.OrderBy(g => g.Code),
            "formationdate" => desc ? query.OrderByDescending(g => g.FormationDate) : query.OrderBy(g => g.FormationDate),
            _ => query.OrderByDescending(g => g.CreatedOn)
        };

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new
            {
                g.Id, g.Code, g.Name, g.BranchCode, g.CollectionDay, g.CollectionTime, g.FormationDate, g.CurrentStatus,
                AreaName = g.AdministrativeUnit.Name,
                g.DesignationId
            })
            .ToListAsync(cancellationToken);

        // Branch names and Credit Officer names are looked up for this page only.
        var pageCodes = rows.Where(r => r.BranchCode.HasValue).Select(r => r.BranchCode!.Value).Distinct().ToList();
        var branchNames = await context.TblOrganizationUnits.AsNoTracking()
            .Where(o => pageCodes.Contains(o.CodeVal) && o.OrganizationUnitType.Name == "Branch")
            .Select(o => new { o.CodeVal, o.Name })
            .ToDictionaryAsync(o => o.CodeVal, o => o.Name, cancellationToken);

        var designationIds = rows.Where(r => r.DesignationId.HasValue).Select(r => r.DesignationId!.Value).Distinct().ToList();
        var officers = await context.TblEmployeeDesignations.AsNoTracking()
            .Where(ed => ed.EffectiveTo == null && designationIds.Contains(ed.DesignationId))
            .Select(ed => new { ed.DesignationId, ed.Employee.FullName })
            .ToListAsync(cancellationToken);
        var officerNames = officers.GroupBy(o => o.DesignationId).ToDictionary(o => o.Key, o => o.First().FullName);

        return Result.Ok(new GroupSearchResponse
        {
            TotalRecords = total,
            Results = rows.Select(r => new GroupListItem
            {
                Id = r.Id,
                Code = r.Code ?? string.Empty,
                Name = r.Name,
                BranchName = r.BranchCode.HasValue && branchNames.TryGetValue(r.BranchCode.Value, out var b) ? b : null,
                AreaName = r.AreaName,
                CreditOfficerName = r.DesignationId.HasValue && officerNames.TryGetValue(r.DesignationId.Value, out var o) ? o : null,
                CollectionDay = r.CollectionDay,
                CollectionDayName = GroupDays.NameOf(r.CollectionDay),
                CollectionTime = r.CollectionTime,
                FormationDate = r.FormationDate,
                Status = (r.CurrentStatus ?? string.Empty).Trim()
            }).ToList()
        });
    }
}
#endregion
