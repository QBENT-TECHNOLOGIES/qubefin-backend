using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Permissions.Queries;

#region --- QUERY ---
public record GetPermissionsQuery : IRequest<Result<List<GetPermissionsResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetPermissionsResponse(string PermissionToken, int DisplayPosition);
#endregion

#region --- HANDLER ---
internal sealed class GetPermissionsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetPermissionsQuery, Result<List<GetPermissionsResponse>>>
{
    public async Task<Result<List<GetPermissionsResponse>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissionTokens = await context
            .TblPermissions
            .AsNoTracking()
            .OrderBy(m => m.DisplayPosition)
            .Select(m => new GetPermissionsResponse(m.PermissionToken, m.DisplayPosition))
            .ToListAsync(cancellationToken);

        return Result.Ok(permissionTokens);
    }
}
#endregion
