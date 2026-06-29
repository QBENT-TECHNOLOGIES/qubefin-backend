using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetUsersQuery : IRequest<Result<List<GetUsersResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetUsersResponse(Guid Id, string UserName, Guid? EmployeeId, string Employee, bool IsActive, bool HasMfaEnabled);
#endregion

#region --- HANDLER ---
internal sealed class GetUsersQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetUsersQuery, Result<List<GetUsersResponse>>>
{
    public async Task<Result<List<GetUsersResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await context
            .TblUsers
            .Include(m => m.Employee)
            .AsNoTracking()
            .OrderBy(m => m.UserName)
            .Select(m => new GetUsersResponse(m.Id, m.UserName, m.EmployeeId, m.Employee != null ? m.Employee.Code + " - " + m.Employee.FirstName : string.Empty, m.IsActive, m.HasMfaEnabled))
            .ToListAsync(cancellationToken);

        return Result.Ok(users);
    }
}
#endregion

