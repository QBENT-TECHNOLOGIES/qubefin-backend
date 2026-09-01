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
        var user = await context
            .TblUsers
            .Include(m => m.Employee.Company)
            .Include(m => m.Employee.OrganizationUnit.OrganizationUnitType)
            .Where(m => m.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            return new RecordNotFoundError($"User not found for the given Id");
        }
        var employeeDesignation = await context.TblEmployeeDesignations
            .Include(m => m.Designation)
            .Where(m => m.EmployeeId == request.EmployeeId)
            .OrderByDescending(m => m.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var response = new UserLoginInfoResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            EmployeeId = user.EmployeeId,
            Employee = user.Employee?.FullName ?? string.Empty,
            Gender = user.Employee?.Gender ?? string.Empty,
            EmployeeCode = user.Employee?.Code ?? string.Empty,
            Designation = employeeDesignation?.Designation?.Name ?? string.Empty,
            CompanyLogoUrl = user.Employee?.Company?.LogoUrl,
            AccessOrganizationUnits = await GetUserOrganizationUnits(user.Employee?.OrganizationUnitId ?? Guid.Empty, cancellationToken)
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
