using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnits.Queries;

#region --- QUERY ---
public record GetOrganizationUnitsWithEmployeesQuery() : IRequest<Result<List<GetOrganizationUnitsWithEmployeesResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetOrganizationUnitsWithEmployeesResponse(Guid Id, string Name, List<OrganizationUnitEmployeeResponse> Employees);

public record OrganizationUnitEmployeeResponse(Guid Id, string Name, string Code, string? CurrentDesignation);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitsWithEmployeesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitsWithEmployeesQuery, Result<List<GetOrganizationUnitsWithEmployeesResponse>>>
{
    public async Task<Result<List<GetOrganizationUnitsWithEmployeesResponse>>> Handle(GetOrganizationUnitsWithEmployeesQuery request, CancellationToken cancellationToken)
    {
        var organizationUnits = await context
            .TblOrganizationUnits
            .OrderBy(m => m.Name)
            .Select(m => new { m.Id, m.Name })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var employees = await context
            .TblEmployees
            .Where(e => e.OrganizationUnitId != null)
            .OrderBy(e => e.FullName)
            .Select(e => new
            {
                OrganizationUnitId = e.OrganizationUnitId!.Value,
                e.Id,
                Name = e.FullName,
                e.Code,
                CurrentDesignation = e.TblEmployeeDesignations
                    .OrderBy(ed => ed.EffectiveTo == null ? 0 : 1)
                    .ThenByDescending(ed => ed.EffectiveFrom)
                    .Select(ed => ed.Designation.Name)
                    .FirstOrDefault()
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var employeesByOrganizationUnit = employees
            .GroupBy(e => e.OrganizationUnitId)
            .ToDictionary(g => g.Key, g => g
                .Select(e => new OrganizationUnitEmployeeResponse(e.Id, e.Name, e.Code, e.CurrentDesignation))
                .ToList());

        var response = organizationUnits
            .Select(m => new GetOrganizationUnitsWithEmployeesResponse(
                m.Id,
                m.Name,
                employeesByOrganizationUnit.TryGetValue(m.Id, out var unitEmployees) ? unitEmployees : []))
            .ToList();

        return Result.Ok(response);
    }
}
#endregion
