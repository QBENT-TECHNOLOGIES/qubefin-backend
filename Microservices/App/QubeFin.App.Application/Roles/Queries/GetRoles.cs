using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Roles.Queries;

#region --- QUERY ---
public record GetRolesQuery : IRequest<Result<List<GetRolesResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetRolesResponse(Guid Id, string Name, bool IsActive);
#endregion

#region --- HANDLER ---
internal sealed class GetRolesQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetRolesQuery, Result<List<GetRolesResponse>>>
{
    public async Task<Result<List<GetRolesResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var users = await context
            .TblRoles
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m => new GetRolesResponse(m.Id, m.Name, m.IsActive))
            .ToListAsync(cancellationToken);

        return Result.Ok(users);
    }
}
#endregion

