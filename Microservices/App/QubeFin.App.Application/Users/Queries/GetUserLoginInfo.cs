using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUserLoginInfoQuery(Guid Id, Guid EmployeeId) : IRequest<Result<GetUserLoginInfoResponse>>;
#endregion

#region --- RESPONSE ---
public record GetUserLoginInfoResponse(Guid Id, string UserName, Guid? EmployeeId, string Employee, string Gender, string EmployeeCode, 
    string Branch, decimal? Latitude, decimal? Longitude,
    TimeOnly? AttendanceInTime, TimeOnly? AttendanceOutTime, int CheckRadiusInMeter, string Designation, string? CompanyLogoUrl);
#endregion

#region --- HANDLER ---
internal sealed class GetUserLoginInfoQueryHandler(QubeFinDataContext context) : IRequestHandler<GetUserLoginInfoQuery, Result<GetUserLoginInfoResponse>>
{
    public async Task<Result<GetUserLoginInfoResponse>> Handle(GetUserLoginInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await context
            .TblUsers
            .Include(m => m.Employee)
            .ThenInclude(m => m.OrganizationUnit)
            .Include(m => m.Employee)
            .ThenInclude(m => m.Company)
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

        var response = new GetUserLoginInfoResponse(
            user.Id,
            user.UserName,
            user.EmployeeId,
            user.Employee?.FullName ?? string.Empty,
            user.Employee?.Gender ?? string.Empty,
            user.Employee?.Code ?? string.Empty,
            user.Employee?.OrganizationUnit?.Name ?? string.Empty,
            user.Employee?.OrganizationUnit?.Latitude,
            user.Employee?.OrganizationUnit?.Longitude,
            user.Employee?.OrganizationUnit?.AttendanceInTime,
            user.Employee?.OrganizationUnit?.AttendanceOutTime,
            user.Employee?.OrganizationUnit?.CheckRadiusInMeter ?? 100,
            employeeDesignation?.Designation?.Name ?? string.Empty,
            user.Employee?.Company?.LogoUrl
        );
        return Result.Ok(response);
    }
}
#endregion
