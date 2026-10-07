using FluentResults;
using MediatR;
using QubeFin.App.Application.Users.Models;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUserLoginInfoQuery(Guid Id, Guid EmployeeId, string? DeviceId) : IRequest<Result<UserLoginInfoResponse>>;
#endregion

#region --- HANDLER ---
internal sealed class GetUserLoginInfoQueryHandler(QubeFinDataContext context) : IRequestHandler<GetUserLoginInfoQuery, Result<UserLoginInfoResponse>>
{
    public async Task<Result<UserLoginInfoResponse>> Handle(GetUserLoginInfoQuery request, CancellationToken cancellationToken)
    {
        var rows = await context.SP_GetUserLoginInfo(request.Id, request.EmployeeId, request.DeviceId);
        var user = rows.FirstOrDefault();
        if (user is null)
        {
            return new RecordNotFoundError($"User not found");
        }
        var response = new UserLoginInfoResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            EmployeeId = user.EmployeeId,
            Employee = user.Employee,
            Gender = user.Gender,
            EmployeeCode = user.EmployeeCode,
            Designation = user.Designation,
            CompanyLogoUrl = user.CompanyLogoUrl,
            IsMileageEnabled = user.IsMileageEnabled,
            AccessOrganizationUnits = rows
                .Where(m => m.OrganizationUnitId.HasValue)
                .Select(m => new UserAccessOrganizationUnit
                {
                    Id = m.OrganizationUnitId!.Value,
                    Name = m.OrganizationUnitName ?? string.Empty,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude,
                    AttendanceInTime = m.AttendanceInTime,
                    AttendanceOutTime = m.AttendanceOutTime,
                    CheckRadiusInMeter = m.CheckRadiusInMeter
                })
                .ToList()
        };
        return Result.Ok(response);
    }
}
#endregion
