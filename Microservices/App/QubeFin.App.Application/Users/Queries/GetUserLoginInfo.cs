using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUserLoginInfoQuery(Guid Id) : IRequest<Result<GetUserLoginInfoResponse>>;
#endregion

#region --- RESPONSE ---
public record GetUserLoginInfoResponse(Guid Id, string UserName, Guid? EmployeeId, string Employee, string Branch, decimal? Latitude, decimal? Longitude, 
    TimeOnly AttendanceInTime, TimeOnly AttendanceOutTime, int CheckRadiusInMeter);
#endregion

#region --- HANDLER ---
internal sealed class GetUserLoginInfoQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUserLoginInfoQuery, Result<GetUserLoginInfoResponse>>
{
    public async Task<Result<GetUserLoginInfoResponse>> Handle(GetUserLoginInfoQuery request, CancellationToken cancellationToken)
    {
        var user = await context
            .TblUsers
            .Include(m => m.Employee)
            .ThenInclude(m => m.OrganizationUnit)
            .Where(m => m.Id == request.Id)
            .Select(m => new GetUserLoginInfoResponse(m.Id, m.UserName, m.EmployeeId, m.Employee == null ? string.Empty : m.Employee.FullName, m.Employee.OrganizationUnit.Name,
                m.Employee.OrganizationUnit.Latitude, m.Employee.OrganizationUnit.Longitude, m.Employee.OrganizationUnit.AttendanceInTime.Value, m.Employee.OrganizationUnit.AttendanceOutTime.Value,
                m.Employee.OrganizationUnit.CheckRadiusInMeter.HasValue ? m.Employee.OrganizationUnit.CheckRadiusInMeter.Value : 100))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return new RecordNotFoundError($"User not found for the given Id");
        }
        return Result.Ok(user);
    }
}
#endregion
