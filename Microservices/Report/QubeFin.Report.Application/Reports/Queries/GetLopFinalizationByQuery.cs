using Microsoft.Extensions.Caching.Memory;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Report.Application.Reports.Models;
using FluentResults;

namespace QubeFin.Report.Application.Reports.Queries;

#region --- QUERY ---
public record GetLopFinalizationByQueryQuery(LopFinalizationReportSearchRequest SearchParam, Guid employeeId) : IRequest<Result<List<LopFinalizationReportResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetLopFinalizationByQueryQueryValidator : AbstractValidator<GetLopFinalizationByQueryQuery>
{
    public GetLopFinalizationByQueryQueryValidator()
    {
        RuleFor(x => x.SearchParam.CompanyId).NotEmpty().WithMessage("Company is required.");
        RuleFor(x => x.SearchParam.Month).NotEmpty().WithMessage("Month is required.").InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");
        RuleFor(x => x.SearchParam.Year).NotEmpty().WithMessage("Year is required.").GreaterThanOrEqualTo(2000).WithMessage("Year must be greater than or equal to 2000.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetLopFinalizationByQueryQueryHandler(QubeFinDataContext context, IMemoryCache cache) : IRequestHandler<GetLopFinalizationByQueryQuery, Result<List<LopFinalizationReportResponse>>>
{
    private const string OrgUnitCacheKey = "org-units-flat";
    private static readonly TimeSpan OrgUnitCacheTtl = TimeSpan.FromMinutes(10);
    public async Task<Result<List<LopFinalizationReportResponse>>> Handle(GetLopFinalizationByQueryQuery request, CancellationToken cancellationToken)
    {
        //bool hasSelectedOrganizationId = request.SearchParam.SearchOrganizationUnitId != null && request.SearchParam.SearchOrganizationUnitId != Guid.Empty;
        //var organizationUnitIds = hasSelectedOrganizationId ? new List<Guid>() : await ResolveOrganizationUnitIdsAsync(request.employeeId, cancellationToken);

        var filterEntitiesQuery = context.TblEmployeeLops
            .Include(e => e.TblEmployeeLopDetails).ThenInclude(e => e.LeaveType)
            .Include(e => e.OrganizationUnit)
            .ThenInclude(e => e.Company)
            .Include(e => e.Employee)
            .Where(e => e.LopYear == request.SearchParam.Year && e.LopMonth == request.SearchParam.Month)
        .AsNoTracking().AsQueryable();

        if (request.SearchParam.CompanyId != null && request.SearchParam.CompanyId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnit.CompanyId == request.SearchParam.CompanyId);
        }
        //if (organizationUnitIds.Any() || hasSelectedOrganizationId)
        //{
        //    if (hasSelectedOrganizationId)
        //    {
        //        filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnitId == request.SearchParam.SearchOrganizationUnitId);
        //    }
        //    else
        //    {
        //        filterEntitiesQuery = filterEntitiesQuery.Where(m => organizationUnitIds.Contains(m.OrganizationUnitId));
        //    }
        //}
        if (request.SearchParam.SearchOrganizationUnitId != null && request.SearchParam.SearchOrganizationUnitId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.OrganizationUnitId == request.SearchParam.SearchOrganizationUnitId);
        }
        if (request.SearchParam.EmployeeId != null && request.SearchParam.EmployeeId != Guid.Empty)
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.EmployeeId == request.SearchParam.EmployeeId);
        }
        if (!string.IsNullOrEmpty(request.SearchParam.SearchText))
        {
            filterEntitiesQuery = filterEntitiesQuery.Where(m => m.Employee.Code!.Contains(request.SearchParam.SearchText.Trim())
            || m.Employee.FullName.Contains(request.SearchParam.SearchText.Trim())
            || m.Employee.OfficialEmail!.Contains(request.SearchParam.SearchText.Trim()));
        }
        if (request.SearchParam.Status > 0)
        {
            filterEntitiesQuery = request.SearchParam.Status switch
            {
                1 => filterEntitiesQuery.Where(m => m.AttendanceIrregularDays > 0),
                2 => filterEntitiesQuery.Where(m => (m.IrregularLopDays + (m.TblEmployeeLopDetails.Where(l => l.LeaveType.Alias == "LOP").Count())) > 0),
                _ => filterEntitiesQuery
            };
        }

        if (request.SearchParam.SortOn is not null && request.SearchParam.SortDirection is not null)
        {
            filterEntitiesQuery = request.SearchParam.SortOn switch
            {
                "code" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Employee.Code) : filterEntitiesQuery.OrderBy(m => m.Employee.Code),
                "name" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.Employee.FullName) : filterEntitiesQuery.OrderBy(m => m.Employee.FullName),
                "company" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.OrganizationUnit.Company.Name) : filterEntitiesQuery.OrderBy(m => m.OrganizationUnit.Company.Name),
                "organizationUnit" => request.SearchParam.SortDirection.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? filterEntitiesQuery.OrderByDescending(m => m.OrganizationUnit.Name) : filterEntitiesQuery.OrderBy(m => m.OrganizationUnit.Name),
                _ => request.SearchParam.SortDirection == "DESC" ? filterEntitiesQuery.OrderByDescending(m => m.Employee.Code) : filterEntitiesQuery.OrderBy(m => m.Employee.Code),
            };
        }

        var result = filterEntitiesQuery.Select(MapToResult).ToList();
        return Result.Ok(result);
    }

    // ---- Mapping ----------------------------------------------------------

    private static LopFinalizationReportResponse MapToResult(TblEmployeeLop m) => new()
    {
        CompanyName = m.OrganizationUnit.Company != null ? m.OrganizationUnit.Company.Name : null,
        EmployeeCode = m.Employee.Code,
        EmployeeName = m.Employee.FullName,
        OrganizationUnitName = m.OrganizationUnit.Name,
        HoliDays = m.HoliDays,
        WorkingDays = m.WorkingDays,
        LeaveDays = m.LeaveDays,
        AttendanceDays = m.AttendanceDays,
        AbsentDays = m.AbsentDays,
        AttendanceIrregularDays = m.AttendanceIrregularDays,
        IrregularLopDays = m.IrregularLopDays,
        IsLocked = m.IsLocked ? "Yes" : "-",
        Remarks = m.Remarks,
    };


    // ---- Organization unit resolution (cached) -----------------------------

    private async Task<List<Guid>> ResolveOrganizationUnitIdsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee?.OrganizationUnitId is not { } rootUnitId)
            return [];

        var branchIds = await GetBranchIdsUnder(rootUnitId, cancellationToken);
        branchIds.Add(rootUnitId);

        return branchIds.Distinct().ToList();
    }

    private async Task<List<Guid>> GetBranchIdsUnder(Guid orgUnitId, CancellationToken cancellationToken)
    {
        var units = await GetAllOrganizationUnitsCachedAsync(cancellationToken);

        var byParent = units
            .Where(u => u.ParentId.HasValue)
            .ToLookup(u => u.ParentId!.Value);

        var result = new List<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(orgUnitId);

        // Iterative traversal avoids recursion-depth issues on deep org trees.
        var visited = new HashSet<Guid>();
        while (stack.Count > 0)
        {
            var currentId = stack.Pop();
            if (!visited.Add(currentId))
                continue;

            var current = units.FirstOrDefault(u => u.Id == currentId);
            if (current == null)
                continue;

            if (current.OrganizationUnitType.Name == "Branch")
                result.Add(current.Id);

            foreach (var child in byParent[currentId])
                stack.Push(child.Id);
        }

        return result;
    }

    private async Task<List<TblOrganizationUnit>> GetAllOrganizationUnitsCachedAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(OrgUnitCacheKey, out List<TblOrganizationUnit>? cached) && cached is not null)
            return cached;

        var units = await context.TblOrganizationUnits
            .Include(u => u.OrganizationUnitType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        cache.Set(OrgUnitCacheKey, units, OrgUnitCacheTtl);
        return units;
    }
}
#endregion