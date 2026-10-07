using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetDevicesByIdQuery(Guid Id) : IRequest<Result<List<GetDevicesByIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetDevicesByIdResponse(Guid Id, string DeviceId, DateTime? AssignDate, bool IsRelease, DateTime? ReleaseDate);
#endregion

#region --- HANDLER ---
internal sealed class GetDevicesByIdQueryHandler(QubeFinDataContext context) : IRequestHandler<GetDevicesByIdQuery, Result<List<GetDevicesByIdResponse>>>
{
    public async Task<Result<List<GetDevicesByIdResponse>>> Handle(GetDevicesByIdQuery request, CancellationToken cancellationToken)
    {
        var devices = await context.TblUserDevices.Where(m => m.UserId == request.Id)
            .OrderBy(m => m.AssignDate)
            .Select(m => new GetDevicesByIdResponse(
                m.Id,
                m.DeviceId,
                m.AssignDate,
                m.IsReleased,
                m.ReleaseDate
                ))
            .ToListAsync(cancellationToken);
        return Result.Ok(devices);

    }
}
#endregion
