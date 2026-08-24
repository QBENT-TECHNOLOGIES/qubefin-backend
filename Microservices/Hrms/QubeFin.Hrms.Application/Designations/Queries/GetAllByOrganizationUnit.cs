using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Designations.Queries;

#region --- QUERY ---
public record GetAllByOrganizationUnitQuery(Guid OrganizationUnitId) : IRequest<Result<List<GetAllByOrganizationUnitResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetAllByOrganizationUnitResponse(Guid Id, string Name, string SalaryGrade, decimal? GrossSalary);
#endregion

#region --- HANDLER ---
internal sealed class GetAllByOrganizationUnitQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetAllByOrganizationUnitQuery, Result<List<GetAllByOrganizationUnitResponse>>>
{
    public async Task<Result<List<GetAllByOrganizationUnitResponse>>> Handle(GetAllByOrganizationUnitQuery request, CancellationToken cancellationToken)
    {
        var designationEntities = await context
         .TblDesignations
         .Include(d => d.OrganizationUnit)
         .Include(d => d.TblDesignationGradeMappings).ThenInclude(dg => dg.Grade).ThenInclude(g => g.TblSalaryStructures)
         .Where(m => m.OrganizationUnitId == request.OrganizationUnitId)
         .AsSplitQuery()
         .OrderBy(m => m.Name)
         .ToListAsync(cancellationToken);

        var designations = await context.TblDesignations
          .Where(m => m.OrganizationUnitId == request.OrganizationUnitId)
          .OrderBy(m => m.Name)
          .Select(m => new
          {
              m.Id,
              m.Name,
              Mapping = m.TblDesignationGradeMappings
                  .OrderByDescending(dg => dg.IsActive)
                  .FirstOrDefault()
          })
          .Select(x => new GetAllByOrganizationUnitResponse(
              x.Id,
              x.Name,
              x.Mapping != null && x.Mapping.Grade != null ? x.Mapping.Grade.Name : string.Empty,
              x.Mapping != null && x.Mapping.Grade != null
                  ? x.Mapping.Grade.TblSalaryStructures
                      .Where(ss => ss.EffectiveToDate == null)
                      .Select(ss => (decimal?)ss.GrossAmount)
                      .FirstOrDefault() ?? 0
                  : 0
          ))
          .AsNoTracking()
          .ToListAsync(cancellationToken);
        return Result.Ok(designations);
    }
}
#endregion

