using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Permissions.Queries;

#region --- QUERY ---
public record GetPermissionsQuery : IRequest<Result<List<GetPermissionsResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetPermissionsResponse(Guid Id, string PermissionToken, string Description, string Icon, string BackgroundClass, string IconClass, int DisplayPosition);
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
            .Select(m => new GetPermissionsResponse(m.Id, m.PermissionToken, m.Description, m.Icon, m.BackgroundClass, m.Icon, m.DisplayPosition))
            .ToListAsync(cancellationToken);

        return Result.Ok(permissionTokens);
    }
}
#endregion
