using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetOrganizationUnitsWiseEmployeesQuery(Guid id) : IRequest<Result<List<OrganizationUnitEmployeeResponse>>>;
#endregion

#region --- RESPONSE ---
public record OrganizationUnitEmployeeResponse(Guid Id, string Name, string Code, string? CurrentDesignation);
#endregion

#region --- HANDLER ---
internal sealed class GetOrganizationUnitsWiseEmployeesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetOrganizationUnitsWiseEmployeesQuery, Result<List<OrganizationUnitEmployeeResponse>>>
{
    public async Task<Result<List<OrganizationUnitEmployeeResponse>>> Handle(GetOrganizationUnitsWiseEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await context
            .TblEmployees
            .Where(e => e.OrganizationUnitId == request.id && e.IsPayrollActive && e.IsActive)
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

        var result = employees.Select(m => new OrganizationUnitEmployeeResponse(m.Id, m.Name, m.Code, m.CurrentDesignation)).ToList();
        return Result.Ok(result);
    }
}
#endregion
