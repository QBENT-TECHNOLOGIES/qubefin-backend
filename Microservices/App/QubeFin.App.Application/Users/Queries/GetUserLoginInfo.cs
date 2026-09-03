using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Application.Users.Models;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUserLoginInfoQuery(Guid Id, Guid EmployeeId) : IRequest<Result<UserLoginInfoResponse>>;
#endregion

#region --- HANDLER ---
internal sealed class GetUserLoginInfoQueryHandler(QubeFinDataContext context) : IRequestHandler<GetUserLoginInfoQuery, Result<UserLoginInfoResponse>>
{
    public async Task<Result<UserLoginInfoResponse>> Handle(GetUserLoginInfoQuery request, CancellationToken cancellationToken)
    {
        var userTask = context.TblUsers
      .AsNoTracking()
      .Include(m => m.Employee!.Company)
      .Include(m => m.Employee!.OrganizationUnit!.OrganizationUnitType)
      .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        var designationTask = context.TblEmployeeDesignations
            .AsNoTracking()
            .Where(m => m.EmployeeId == request.EmployeeId)
            .OrderByDescending(m => m.EffectiveFrom)
            .Select(m => m.Designation!.Name)
            .FirstOrDefaultAsync(cancellationToken);

        await Task.WhenAll(userTask, designationTask);

        var user = await userTask;
        var designationName = await designationTask;

        if (user is null)
        {
            return new RecordNotFoundError($"User not found for the given Id: {request.Id}");
        }

        var accessOrganizationUnits = user.Employee?.OrganizationUnit?.Latitude != null ?
             new List<UserAccessOrganizationUnit>
           {
              new UserAccessOrganizationUnit
              {
                  Id = user.Employee.OrganizationUnit.Id,
                  Name = user.Employee.OrganizationUnit.Name,
                  Latitude = user.Employee.OrganizationUnit.Latitude,
                  Longitude = user.Employee.OrganizationUnit.Longitude,
                  AttendanceInTime = user.Employee.OrganizationUnit.AttendanceInTime,
                  AttendanceOutTime = user.Employee.OrganizationUnit.AttendanceOutTime,
                  CheckRadiusInMeter = user.Employee.OrganizationUnit.CheckRadiusInMeter ?? 100
              }
           }
         : await GetUserOrganizationUnits(user.Employee?.OrganizationUnitId ?? Guid.Empty, cancellationToken);

        var response = new UserLoginInfoResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            EmployeeId = user.EmployeeId,
            Employee = user.Employee?.FullName ?? string.Empty,
            Gender = user.Employee?.Gender ?? string.Empty,
            EmployeeCode = user.Employee?.Code ?? string.Empty,
            Designation = designationName ?? string.Empty,
            CompanyLogoUrl = user.Employee?.Company?.LogoUrl,
            AccessOrganizationUnits = accessOrganizationUnits
        };
        return Result.Ok(response);
    }
    private async Task<List<UserAccessOrganizationUnit>> GetUserOrganizationUnits(Guid orgUnitId, CancellationToken cancellationToken)
    {
        var units = await context.TblOrganizationUnits.Include(u => u.OrganizationUnitType).ToListAsync(cancellationToken);
        IEnumerable<TblOrganizationUnit> Traverse(Guid id)
        {
            var current = units.FirstOrDefault(u => u.Id == id);
            if (current == null) yield break;

            if (current.OrganizationUnitType.Name == "Branch")
                yield return current;

            foreach (var child in units.Where(u => u.ParentId == id))
                foreach (var descendant in Traverse(child.Id))
                    yield return descendant;
        }
        return Traverse(orgUnitId)
        .Distinct()
        .Select(b => new UserAccessOrganizationUnit
        {
            Id = b.Id,
            Name = b.Name,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            AttendanceInTime = b.AttendanceInTime,
            AttendanceOutTime = b.AttendanceOutTime,
            CheckRadiusInMeter = b.CheckRadiusInMeter ?? 100
        }).ToList();
    }
}
#endregion
